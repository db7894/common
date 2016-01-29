package com.bashwork.commons.workflow;

import static com.bashwork.commons.utility.Validate.notNull;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.bashwork.commons.serialize.Serializer;
import com.bashwork.commons.serialize.Serializers;
import com.bashwork.commons.serialize.string.JsonStringSerializer;
import com.bashwork.commons.supplier.ConfirmedSupplier;
import com.bashwork.commons.utility.FileUtility;
import com.google.common.util.concurrent.Service;
import com.google.common.util.concurrent.ServiceManager;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * This wraps all the management of the WorkflowService instances
 * and exposes a simple API for creating and stopping the overall
 * workflow system.
 */
public final class WorkflowManager {

    static final Logger logger = LogManager.getLogger(FileUtility.class);

    private final ServiceManager manager;

    /**
     * Initialize a new instance of the WorkflowManager.
     *
     * @param manager The underlying manager to operate with.
     */
    private WorkflowManager(ServiceManager manager) {
        this.manager = notNull(manager, "manager");
    }

    /**
     * Starts up the workflow system asynchronously.
     */
    public void start() {
        logger.info("starting up the workflow system");
        manager.startAsync();
    }

    /**
     * Check if the underlying service manager is running correctly.
     *
     * @return true if running correctly, false otherwise.
     */
    public boolean isRunning() {
        return manager.isHealthy();
    }

    /**
     * Shuts down the workflow system asynchronously.
     *
     * TODO does it make sense to stop the step worker from
     * accepting new work? This will cause all currently executing
     * workflows to jump to the onError state allowing them to fail
     * gracefully. Otherwise, we will wait until all currently running
     * workflows are finished (correct).
     */
    public void stop() {
        logger.info("shutting down the workflow system");
        manager.stopAsync();
    }

    /**
     * Create a new instance of the builder used to create
     * this type.
     *
     * @return A new initialized builder instance.
     */
    public static Builder builder() {
        return new Builder();
    }

    public static final class Builder {
        private Builder() { }

        private int workers;
        private Serializer<String> serializer;
        private ConfirmedSupplier<WorkflowRequest> supplier;
        private Iterable<Workflow<?>> workflows;

        public Builder withWorkers(int workers) {
            this.workers = workers;
            return this;
        }

        public Builder withSerializer(Serializer<String> serializer) {
            this.serializer = serializer;
            return this;
        }

        public Builder withDefaultSerializer() {
            return withSerializer(new JsonStringSerializer());
        }

        public Builder withSupplier(ConfirmedSupplier<WorkflowRequest> supplier) {
            this.supplier = supplier;
            return this;
        }

        public Builder withWorkflows(Iterable<Workflow<?>> workflows) {
            this.workflows = workflows;
            return this;
        }

        public WorkflowManager build() {
            Map<String, WorkflowExecutor<?>> executors = getWorkflows();
            ServiceManager manager = new ServiceManager(getServices(executors));
            logger.info("successfully constructed workflow system");
            return new WorkflowManager(manager);
        }

        /**
         * Given a collection of compiled workflow executors, generate the supplied
         * number of WorkflowService instances.
         *
         * @param executors The collection of compiled WorkflowExecutors
         * @return A collection of WorkflowService instances.
         */
        private Iterable<Service> getServices(Map<String, WorkflowExecutor<?>> executors) {
            List<Service> services = new ArrayList<Service>();

            logger.info("worklow creating {} worker services", workers);
            for (int i = 0; i < workers; ++i) {
                services.add(new WorkflowService(supplier, executors));
            }

            return services;
        }

        /**
         * Generate a lookup map that converts from the workflow name
         * to the workflow implementation. Most of this is just magic
         * to wire everything together in a type-safe way.
         *
         * @return The registered workflows.
         */
        @SuppressWarnings({ "rawtypes", "unchecked" })
        private Map<String, WorkflowExecutor<?>> getWorkflows() {
            Map<String, WorkflowExecutor<?>> registered = new HashMap<>();

            for (Workflow<?> workflow : workflows) {
                logger.info("registering workflow {}", workflow.getName());
                WorkflowExecutor<?> toRegister = new WorkflowExecutor(workflow,
                    Serializers.toSpecific(serializer, workflow.getType()));
                registered.put(workflow.getName(), toRegister);
            }

            return registered;
        }
    }
}

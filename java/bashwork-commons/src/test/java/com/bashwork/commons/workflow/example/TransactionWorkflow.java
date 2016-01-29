package com.bashwork.commons.workflow.example;

import java.util.Arrays;
import java.util.List;
import java.util.concurrent.Executors;

import com.bashwork.commons.activity.NonRetryableException;
import com.bashwork.commons.activity.RetryableException;
import com.bashwork.commons.notification.NotifierService;
import com.bashwork.commons.workflow.SplitWorkflow;
import com.bashwork.commons.workflow.WorkflowResult;
import com.bashwork.commons.workflow.WorkflowStatus;
import com.bashwork.commons.retry.RetryStrategy;
import com.bashwork.commons.retry.strategy.RetryStrategies;

public class TransactionWorkflow extends SplitWorkflow<Transaction> {

    private TransactionBank bank;
    private NotifierService notifier;

    //private static final Throwable defaultEx = new Throwable(TransactionMessage.Unknown);
    private static final RetryStrategy<Void> strategy = new RetryStrategies
        .ExponentialBackoffBuilder()
        .withInitialIntervalMillis(1000)
        .withExponentialFactor(2)
        .withMaxAttempts(1)
        .withMaxElapsedTimeMillis(5000)
        .build();

    /**
     * Initialize a new instance of the TransactionWorkflow class
     *
     * @param bank The bank to run the transactions against.
     * @param notifier The notifier to send the result to.
     */
    public TransactionWorkflow(TransactionBank bank, NotifierService notifier) {
        super(Executors.newSingleThreadScheduledExecutor(), strategy);

        this.bank = bank;
        this.notifier = notifier;
    }

    // ------------------------------------------------------------------------
    // Workflow Interface
    // ------------------------------------------------------------------------

    @Override
    public Class<Transaction> getType() {
        return Transaction.class;
    }

    @Override
    public List<Transaction> onSplit(Transaction request) {
        return Arrays.asList(request);
    }

    @Override
    public void onComplete(Transaction request, List<WorkflowResult<Transaction>> results) {
        final WorkflowResult<Transaction> result = results.get(0);
        final String address = request.getDebitor();
        final String message = (result.getStatus() == WorkflowStatus.SUCCESS)
            ? TransactionMessage.Success
                : result.getMessage();

        notifier.notify(address, message);
    }

    @Override
    public void onExecute(Transaction request) throws NonRetryableException, RetryableException {
        bank.debit(request.getDebitor(), request.getAmount());
        bank.credit(request.getCreditor(), request.getAmount());
    }
}

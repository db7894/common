package com.bashwork.commons.workflow.example;

import java.util.Arrays;
import java.util.List;
import java.util.concurrent.Executors;

import com.bashwork.commons.notification.NotifierService;
import com.bashwork.commons.workflow.SplitWorkflow;
import com.bashwork.commons.workflow.WorkflowResult;
import com.bashwork.commons.workflow.WorkflowStatus;

public class TransactionWorkflow extends SplitWorkflow<Transaction> {

    private TransactionBank bank;
    private NotifierService notifier;

    /**
     * Initialize a new instance of the TransactionWorkflow class
     *
     * @param bank The bank to run the transactions against.
     * @param notifier The notifier to send the result to.
     */
    public TransactionWorkflow(TransactionBank bank, NotifierService notifier) {
        super(Executors.newSingleThreadScheduledExecutor());

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
    public void onExecute(Transaction request) {
        bank.debit(request.getDebitor(), request.getAmount());
        bank.credit(request.getCreditor(), request.getAmount());
    }
}

package com.bashwork.commons.workflow;

import static org.hamcrest.Matchers.equalTo;
import static org.junit.Assert.assertThat;

import java.math.BigDecimal;
import java.util.UUID;
import java.util.function.Consumer;

import org.junit.Test;

import com.bashwork.commons.notification.testing.InProcessNotifierService;
import com.bashwork.commons.workflow.example.Transaction;
import com.bashwork.commons.workflow.example.TransactionBank;
import com.bashwork.commons.workflow.example.TransactionMessage;
import com.bashwork.commons.workflow.example.TransactionWorkflow;
import com.bashwork.commons.workflow.testing.BaseWorkflowTest;

/**
 * This is an example of testing a workflow locally.
 */
public class WorkflowManagerTest extends BaseWorkflowTest {

    private static final String creditor = UUID.randomUUID().toString();
    private static final String debitor = UUID.randomUUID().toString();

    @Test
    public void test_valid_transaction() {
        final TransactionBank bank = new TransactionBank(creditor, debitor);
        final BigDecimal amount = new BigDecimal("22.25");
        final Transaction transaction = new Transaction(creditor, debitor, amount);

        bank.credit(creditor, new BigDecimal("50.00"));
        bank.credit(debitor, new BigDecimal("22.25"));

        execute_transaction(transaction, bank, result -> {
            assertThat(result, equalTo(TransactionMessage.Success));
        });
    }

    @Test
    public void test_missing_creditor_account() {
        final TransactionBank bank = new TransactionBank(debitor);
        final BigDecimal amount = new BigDecimal("22.25");
        final Transaction transaction = new Transaction(creditor, debitor, amount);

        bank.credit(debitor, new BigDecimal("22.25"));

        execute_transaction(transaction, bank, result -> {
            assertThat(result, equalTo(TransactionMessage.MissingAccount));
        });
    }

    @Test
    public void test_missing_debitor_account() {
        final TransactionBank bank = new TransactionBank(creditor);
        final BigDecimal amount = new BigDecimal("22.25");
        final Transaction transaction = new Transaction(creditor, debitor, amount);

        bank.credit(creditor, new BigDecimal("50.00"));

        execute_transaction(transaction, bank, result -> {
            assertThat(result, equalTo(TransactionMessage.MissingAccount));
        });
    }

    @Test
    public void test_insufficient_funds() {
        final TransactionBank bank = new TransactionBank(creditor, debitor);
        final BigDecimal amount = new BigDecimal("22.25");
        final Transaction transaction = new Transaction(creditor, debitor, amount);

        bank.credit(debitor, new BigDecimal("20.00"));
        bank.credit(creditor, new BigDecimal("50.00"));

        execute_transaction(transaction, bank, result -> {
            assertThat(result, equalTo(TransactionMessage.InsufficientFunds));
        });
    }

    // ------------------------------------------------------------------------
    // Workflow Integration Helpers
    // ------------------------------------------------------------------------

    /**
     * Given a transaction to apply to the supplied bank, run the workflow
     * and then execute the verification step when the workflow is complete.
     *
     * @param transaction The transaction to execute.
     * @param bank The bank to execute the transaction against.
     * @param verifier The verification step to run on the workflow result.
     */
    private static void execute_transaction(Transaction transaction, TransactionBank bank,
        Consumer<String> verifier) {

        final InProcessNotifierService notifier = new InProcessNotifierService();
        final TransactionWorkflow workflow = new TransactionWorkflow(bank, notifier);

        executeWorkflow(transaction, workflow, notifier.register(debitor)
            .thenAccept(verifier));
    }
}

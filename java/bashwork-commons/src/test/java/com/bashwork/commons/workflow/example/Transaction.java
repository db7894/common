package com.bashwork.commons.workflow.example;

import java.math.BigDecimal;

public class Transaction {

    private final String creditor;
    private final String debitor;
    private final BigDecimal amount;

    public Transaction(String creditor, String debitor, BigDecimal amount) {
        this.creditor = creditor;
        this.debitor = debitor;
        this.amount = amount;
    }

    public String getCreditor() {
        return creditor;
    }

    public String getDebitor() {
        return debitor;
    }

    public BigDecimal getAmount() {
        return amount;
    }
}

package com.bashwork.commons.workflow.example;

public final class TransactionMessage {
    private TransactionMessage() {}

    public static final String Success = "success";
    public static final String Unknown = "unknown error";
    public static final String MissingAccount = "missing account";
    public static final String InsufficientFunds = "insufficient funds";
}

package com.bashwork.commons.workflow.example;

import java.math.BigDecimal;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;

public class TransactionBank {

    public Map<String, BigDecimal> accounts = new ConcurrentHashMap<>();

    public TransactionBank(String... initials) {
        for (String initial : initials) {
            accounts.put(initial, BigDecimal.ZERO);
        }
    }

    public Set<String> getAccounts() {
        return accounts.keySet();
    }

    public BigDecimal debit(String account, final BigDecimal amount) {
        return accounts.compute(account, (key, current) -> {
            if (current == null) {
                throw new IllegalStateException(TransactionMessage.MissingAccount);
            }
            if (current.compareTo(amount) < 0) {
                throw new IllegalStateException(TransactionMessage.InsufficientFunds);
            }
            return current.subtract(amount);
        });
    }

    public BigDecimal credit(String account, final BigDecimal amount) {
        return accounts.compute(account, (key, current) -> {
            if (current == null) {
                throw new IllegalStateException(TransactionMessage.MissingAccount);
            }
            return current.add(amount);
        });
    }
}

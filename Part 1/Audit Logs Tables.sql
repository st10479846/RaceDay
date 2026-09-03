-- =============================================
-- 8. Audit Logs Table (for tracking changes)
-- =============================================



CREATE TABLE AuditLogs (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NULL,
    action VARCHAR(50) NOT NULL,
    table_name VARCHAR(50) NOT NULL,
    record_id INT NULL,
    old_values TEXT NULL,
    new_values TEXT NULL,
    ip_address VARCHAR(45) NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE SET NULL
);
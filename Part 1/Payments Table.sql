-- =============================================
-- 7. Payments Table
-- =============================================


CREATE TABLE Payments (
    payment_id INT AUTO_INCREMENT PRIMARY KEY,
    enrol_id INT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    payment_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    method VARCHAR(20) NOT NULL CHECK (method IN ('Credit Card', 'Debit Card', 'EFT', 'Cash', 'Other')),
    status VARCHAR(20) DEFAULT 'Pending' CHECK (status IN ('Pending', 'Completed', 'Failed', 'Refunded')),
    reference VARCHAR(100) NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT FK_Payments_Enrolments FOREIGN KEY (enrol_id) REFERENCES Enrolments(enrol_id) ON DELETE CASCADE
);
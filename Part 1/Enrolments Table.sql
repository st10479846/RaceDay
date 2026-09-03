-- =============================================
-- 5. Enrolments Table
-- =============================================

CREATE TABLE Enrolments (
    enrol_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    event_id INT NOT NULL,
    category_id INT NOT NULL,
    status VARCHAR(20) DEFAULT 'Pending' CHECK (status IN ('Pending', 'Confirmed', 'CheckedIn', 'Completed', 'Withdrawn')),
    entry_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    start_time TIME NULL,
    finish_time TIME NULL,
    bib_number VARCHAR(20) NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    waiver_signed BOOLEAN DEFAULT FALSE,
    emergency_contact VARCHAR(100) NULL,
    emergency_phone VARCHAR(20) NULL,
    CONSTRAINT FK_Enrolments_Users FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE,
    CONSTRAINT FK_Enrolments_Events FOREIGN KEY (event_id) REFERENCES Events(event_id) ON DELETE CASCADE,
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (category_id) REFERENCES Categories(category_id),
    CONSTRAINT UQ_Enrolment_User_Event UNIQUE (user_id, event_id)
);
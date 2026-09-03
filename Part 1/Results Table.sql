-- =============================================
-- 6. Results Table
-- =============================================

CREATE TABLE Results (
    result_id INT AUTO_INCREMENT PRIMARY KEY,
    enrol_id INT NOT NULL,
    category_id INT NOT NULL,
    finish_time TIME NOT NULL,
    position INT NULL,
    age_category VARCHAR(20) NULL,
    gender_category VARCHAR(10) NULL,
    certificate_url VARCHAR(255) NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    is_verified BOOLEAN DEFAULT FALSE,
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (enrol_id) REFERENCES Enrolments(enrol_id) ON DELETE CASCADE,
    CONSTRAINT FK_Results_Categories FOREIGN KEY (category_id) REFERENCES Categories(category_id)
);
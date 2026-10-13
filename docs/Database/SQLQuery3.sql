-- =============================================
-- 2. Events Table
-- =============================================
CREATE TABLE Events (
    event_id INT IDENTITY(1,1) PRIMARY KEY,
    organizer_id INT NOT NULL,
    title VARCHAR(200) NOT NULL,
    description TEXT NULL,
    start_date DATETIME NOT NULL,
    end_date DATETIME NOT NULL,
    location VARCHAR(200) NOT NULL,
    status VARCHAR(20) DEFAULT 'Draft' CHECK (status IN ('Draft', 'Open', 'Closed', 'Completed')),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    banner_image_url VARCHAR(255) NULL,
    CONSTRAINT FK_Events_Users FOREIGN KEY (organizer_id) REFERENCES Users(user_id) ON DELETE CASCADE
);
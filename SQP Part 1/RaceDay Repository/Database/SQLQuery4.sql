-- =============================================
-- 3. Categories Table
-- =============================================
CREATE TABLE Categories (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT NULL,
    distance_km DECIMAL(5,2) NOT NULL,
    entry_fee DECIMAL(10,2) NOT NULL,
    start_time TIME NOT NULL,
    max_participants INT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME NULL,
    is_active BIT DEFAULT 1,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (event_id) REFERENCES Events(event_id) ON DELETE CASCADE
);
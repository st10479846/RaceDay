-- =============================================
-- 1. Users Table
-- =============================================


CREATE TABLE Users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('Organiser', 'Participant')),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    last_login DATETIME NULL,
    is_active BOOLEAN DEFAULT TRUE,
    phone_number VARCHAR(20) NULL,
    profile_image_url VARCHAR(255) NULL
);
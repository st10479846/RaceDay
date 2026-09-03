-- =============================================
-- Seed Data
-- =============================================

-- Insert Organisers
INSERT INTO Users (email, password_hash, full_name, role, phone_number) VALUES
('john.doe@events.co.za', 'hashed_password_1', 'John Doe', 'Organiser', '+27 82 123 4567'),
('jane.smith@events.co.za', 'hashed_password_2', 'Jane Smith', 'Organiser', '+27 83 234 5678');

-- Insert Participants
INSERT INTO Users (email, password_hash, full_name, role, phone_number) VALUES
('runner1@gmail.com', 'hashed_password_3', 'Thabo Mokoena', 'Participant', '+27 71 345 6789'),
('runner2@gmail.com', 'hashed_password_4', 'Lindiwe Nkosi', 'Participant', '+27 72 456 7890'),
('runner3@gmail.com', 'hashed_password_5', 'Mike Johnson', 'Participant', '+27 73 567 8901'),
('runner4@gmail.com', 'hashed_password_6', 'Sarah Williams', 'Participant', '+27 74 678 9012');

-- Insert Events
INSERT INTO Events (organizer_id, title, description, start_date, end_date, location, status, banner_image_url) VALUES
(1, 'Cape Town Cycle Tour 2026', 'The world''s largest timed cycle race', '2026-03-08 06:00:00', '2026-03-08 18:00:00', 'Cape Town, Western Cape', 'Open', 'https://example.com/banners/ctct2026.jpg'),
(1, 'Comrades Marathon 2026', 'The ultimate human race - 90km ultra marathon', '2026-06-16 05:30:00', '2026-06-16 17:30:00', 'Pietermaritzburg to Durban, KZN', 'Open', 'https://example.com/banners/comrades2026.jpg'),
(2, 'Soweto Marathon 2026', 'Experience the spirit of Soweto', '2026-11-04 06:00:00', '2026-11-04 14:00:00', 'Soweto, Johannesburg', 'Draft', 'https://example.com/banners/soweto2026.jpg'),
(2, 'Two Oceans Marathon 2026', 'Africa''s most beautiful marathon', '2026-04-17 05:00:00', '2026-04-17 16:00:00', 'Cape Town, Western Cape', 'Open', 'https://example.com/banners/twooceans2026.jpg');

-- Insert Categories
INSERT INTO Categories (event_id, name, description, distance_km, entry_fee, start_time, max_participants) VALUES
-- Cape Town Cycle Tour Categories
(1, 'Elite Men', 'Professional male cyclists', 109.0, 850.00, '06:00:00', 500),
(1, 'Elite Women', 'Professional female cyclists', 109.0, 850.00, '06:05:00', 300),
(1, 'Amateur Open', 'Amateur cyclists', 109.0, 450.00, '06:10:00', 8000),
(1, 'Fun Ride', 'Recreational cyclists', 70.0, 250.00, '06:30:00', 5000),

-- Comrades Marathon Categories
(2, 'Open Men', 'All male runners', 90.0, 1200.00, '05:30:00', 8000),
(2, 'Open Women', 'All female runners', 90.0, 1200.00, '05:30:00', 5000),
(2, 'Veterans 40+', 'Runners 40 years and older', 90.0, 1000.00, '05:30:00', 3000),
(2, 'Corporate Challenge', 'Corporate teams', 45.0, 1500.00, '06:00:00', 1000),

-- Soweto Marathon Categories
(3, 'Full Marathon', '42.2km run', 42.2, 500.00, '06:00:00', 5000),
(3, 'Half Marathon', '21.1km run', 21.1, 350.00, '06:15:00', 8000),
(3, '10km Fun Run', '10km community run', 10.0, 200.00, '06:30:00', 10000),

-- Two Oceans Marathon Categories
(4, 'Ultra Marathon', '56km ultra marathon', 56.0, 950.00, '05:00:00', 5000),
(4, 'Half Marathon', '21.1km run', 21.1, 550.00, '05:30:00', 8000);

-- Insert Event Routes
INSERT INTO EventRoutes (event_id, route_name, start_point, end_point, distance_km, altitude_gain, gpx_track_url) VALUES
(1, 'CTCT Main Route', 'Mouille Point, Cape Town', 'Mouille Point, Cape Town', 109.0, 500, 'https://example.com/gpx/ctct2026.gpx'),
(2, 'Comrades Up Run', 'Pietermaritzburg City Hall', 'Kings Park Stadium, Durban', 90.0, 850, 'https://example.com/gpx/comrades2026_up.gpx'),
(4, 'Two Oceans Ultra', 'Main Road, Cape Town', 'University of Cape Town', 56.0, 780, 'https://example.com/gpx/twooceans2026.gpx');

-- Insert Enrolments
INSERT INTO Enrolments (user_id, event_id, category_id, status, bib_number, waiver_signed, emergency_contact, emergency_phone) VALUES
(3, 1, 3, 'Confirmed', 'C2026-001', 1, 'Maria Mokoena', '+27 71 345 6790'),
(4, 1, 4, 'Pending', 'C2026-002', 0, 'Sipho Nkosi', '+27 72 456 7891'),
(5, 2, 5, 'Confirmed', 'COM-001', 1, 'Emma Johnson', '+27 73 567 8902'),
(6, 4, 12, 'Confirmed', 'TWO-001', 1, 'David Williams', '+27 74 678 9013'),
(3, 4, 13, 'Pending', NULL, 0, 'Maria Mokoena', '+27 71 345 6790');

-- Insert Results
INSERT INTO Results (enrol_id, category_id, finish_time, position, age_category, gender_category, certificate_url, is_verified) VALUES
(1, 3, '03:45:30', 150, '20-29', 'Male', 'https://example.com/certificates/C2026-001.pdf', 1),
(3, 5, '07:15:45', 250, '30-39', 'Male', 'https://example.com/certificates/COM-001.pdf', 1),
(4, 12, '05:30:15', 180, '20-29', 'Female', 'https://example.com/certificates/TWO-001.pdf', 0);

-- Insert Payments
INSERT INTO Payments (enrol_id, amount, method, status, reference) VALUES
(1, 450.00, 'Credit Card', 'Completed', 'PAY-001'),
(2, 250.00, 'Debit Card', 'Pending', 'PAY-002'),
(3, 1200.00, 'EFT', 'Completed', 'PAY-003'),
(4, 950.00, 'Credit Card', 'Completed', 'PAY-004');
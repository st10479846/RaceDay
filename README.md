# RaceDay Event Management System

## Project Description
RaceDay is a full-stack web-based event management system designed specifically for the South African road running, walking, and cycling community. The platform solves the problem of manual, paper-based event management by providing a unified digital solution for event organisers to create and manage events, while allowing participants to browse, enter, and track their performance history.

## User Roles

### Organiser
- Create, edit, and delete events
- Manage event categories (e.g., 5km, 10km, Half Marathon)
- Capture and update participant results
- View all event enrolments and participant lists
- Access event statistics and reports
- Manage event details including dates, locations, and routes

### Participant
- Create an account and manage profile
- Browse upcoming events
- Enter events by selecting a category
- View personal enrolment history
- Track race results and personal best times
- Access event details, routes, and information

## Part 1

This repository contains the database design and planning documentation for the RaceDay system:

### 1. Entity Relationship Diagram (ERD)
- **File**: `docs/RaceDay_ERD.jpg`
- The ERD illustrates the database schema showing all tables, relationships, and primary/foreign keys
- Tables include: Events, Participants, Categories, Enrolments, Results, and Payments

### 2. API Endpoint Plan
- **File**: `docs/API Endpoint Plan ST10479846.pdf`
- Comprehensive API documentation outlining all RESTful endpoints
- Includes request/response formats for: Events, Participants, Categories, Enrolments, Results, and Payments
- Details HTTP methods (GET, POST, PUT, DELETE) for each endpoint

### 3. SQL Database Script
- **Location**: `/Database` folder
- Contains all SQL scripts for creating the RaceDay database
- Includes table creation, stored procedures, views, and functions

## Repository Structure

RaceDay/
├── Database/
│ ├── SQLQuery1.sql - Database schema
│ ├── SQLQuery2.sql - Table creation scripts
│ ├── SQLQuery3.sql - Stored procedures
│ ├── SQLQuery4.sql - Views and functions
│ ├── SQLQuery5-10.sql - Additional database objects
├── docs/
│ ├── RaceDay_ERD.jpg - Entity Relationship Diagram
│ └── API Endpoint Plan ST10479846.pdf - API documentation
└── github/
└── Flow Chart.ST10479846.pdf - System flow diagram
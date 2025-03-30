-- Creare baza de date
CREATE DATABASE MPIDatabase;
GO

-- Utilizare baza de date
USE MPIDatabase;
GO

-- Users table (Teachers & Students)
CREATE TABLE Users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    first_name NVARCHAR(50) NOT NULL,
    last_name NVARCHAR(50) NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    password NVARCHAR(MAX) NOT NULL,
    role NVARCHAR(10) CHECK (role IN ('teacher', 'student')) NOT NULL,
    created_at DATETIME DEFAULT GETDATE()
);

-- Courses table
CREATE TABLE Courses (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX),
    teacher_id INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (teacher_id) REFERENCES Users(id) ON DELETE SET NULL
);

-- Enrollments table (Mapping students to courses)
CREATE TABLE Enrollments (
    id INT IDENTITY(1,1) PRIMARY KEY,
    student_id INT NOT NULL,
    course_id INT NOT NULL,
    enrolled_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (student_id) REFERENCES Users(id) ON DELETE SET NULL,
    FOREIGN KEY (course_id) REFERENCES Courses(id) ON DELETE SET NULL,
    UNIQUE (student_id, course_id)
);

-- Grades table (With history tracking)
CREATE TABLE Grades (
    id INT IDENTITY(1,1) PRIMARY KEY,
    student_id INT NOT NULL,
    course_id INT NOT NULL,
    grade DECIMAL(5,2) CHECK (grade >= 0 AND grade <= 10),
    graded_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (student_id) REFERENCES Users(id) ON DELETE SET NULL,
    FOREIGN KEY (course_id) REFERENCES Courses(id) ON DELETE SET NULL
);

-- Insert Teachers
INSERT INTO Users (first_name, last_name, email, password, role) VALUES
('John', 'Doe', 'john.doe@example.com', 'password123', 'teacher'),
('Jane', 'Smith', 'jane.smith@example.com', 'password456', 'teacher');

-- Insert Students
INSERT INTO Users (first_name, last_name, email, password, role) VALUES
('Alice', 'Brown', 'alice.brown@example.com', 'studentpass1', 'student'),
('Bob', 'Green', 'bob.green@example.com', 'studentpass2', 'student'),
('Charlie', 'Black', 'charlie.black@example.com', 'studentpass3', 'student'),
('David', 'White', 'david.white@example.com', 'studentpass4', 'student'),
('Ella', 'Johnson', 'ella.johnson@example.com', 'studentpass5', 'student'),
('Frank', 'Adams', 'frank.adams@example.com', 'studentpass6', 'student'),
('Grace', 'Clark', 'grace.clark@example.com', 'studentpass7', 'student'),
('Henry', 'Moore', 'henry.moore@example.com', 'studentpass8', 'student'),
('Ivy', 'Turner', 'ivy.turner@example.com', 'studentpass9', 'student'),
('Jack', 'Scott', 'jack.scott@example.com', 'studentpass10', 'student');

-- Insert Courses
INSERT INTO Courses (name, description, teacher_id) VALUES
('Mathematics', 'Advanced mathematics course', 1),
('Physics', 'Fundamentals of physics', 2);

-- Enroll Students in Courses
INSERT INTO Enrollments (student_id, course_id) VALUES
(3, 1), (4, 1), (5, 1), (6, 1), (7, 1),
(8, 2), (9, 2), (10, 2), (11, 2), (12, 2);

-- Insert Grades
INSERT INTO Grades (student_id, course_id, grade) VALUES
(3, 1, 8.5), (4, 1, 9.0), (5, 1, 7.5), (6, 1, 8.0), (7, 1, 9.5),
(8, 2, 7.0), (9, 2, 8.5), (10, 2, 9.0), (11, 2, 7.8), (12, 2, 8.2);
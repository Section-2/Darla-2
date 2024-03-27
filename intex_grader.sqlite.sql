USE intexerd;

CREATE TABLE IF NOT EXISTS permission (
    permission_type INT NOT NULL,
    permission_description VARCHAR(255) NOT NULL,
    PRIMARY KEY(permission_type)
);

CREATE TABLE IF NOT EXISTS peer_evaluation_question (
    question_id INT NOT NULL,
    question VARCHAR(255) NOT NULL,
    PRIMARY KEY(question_id)
);

CREATE TABLE IF NOT EXISTS room (
    room_id INT NOT NULL,
    room_name VARCHAR(255) NOT NULL,
    PRIMARY KEY(room_id)
);

CREATE TABLE IF NOT EXISTS user_password (
    user_id INT NOT NULL,
    user_password VARCHAR(255) NOT NULL,
    PRIMARY KEY(user_id)
    -- Foreign key constraint will be added after 'user' table creation
);

CREATE TABLE IF NOT EXISTS user (
    user_id INT NOT NULL AUTO_INCREMENT,
    net_id VARCHAR(255) NOT NULL,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    permission_type INT NOT NULL,
    PRIMARY KEY(user_id),
    FOREIGN KEY(permission_type) REFERENCES permission(permission_type)
);

-- Now we can add the foreign key to user_password
ALTER TABLE user_password ADD CONSTRAINT fk_user_password_user_id FOREIGN KEY (user_id) REFERENCES user(user_id);

CREATE TABLE IF NOT EXISTS team (
    team_number INT NOT NULL,
    PRIMARY KEY(team_number)
);

CREATE TABLE IF NOT EXISTS student_team (
    user_id INT NOT NULL,
    team_number INT NOT NULL,
    PRIMARY KEY(user_id),
    FOREIGN KEY(user_id) REFERENCES user(user_id),
    FOREIGN KEY(team_number) REFERENCES team(team_number)
);
CREATE TABLE IF NOT EXISTS rubric (
    assignment_id INT NOT NULL AUTO_INCREMENT,
    class_code INT NOT NULL,
    subcategory VARCHAR(255),
    description VARCHAR(255),
    max_points INT,
    instructor_notes VARCHAR(255),
    PRIMARY KEY(assignment_id)
);
CREATE TABLE IF NOT EXISTS grade (
    grade_id INT NOT NULL AUTO_INCREMENT,
    assignment_id INT NOT NULL,
    user_id INT NOT NULL,
    team_number INT NOT NULL,
    points_earned DECIMAL(10,2) NOT NULL,
    comments VARCHAR(255),
    PRIMARY KEY(grade_id),
    FOREIGN KEY(assignment_id) REFERENCES rubric(assignment_id),
    FOREIGN KEY(user_id) REFERENCES student_team(user_id),
    FOREIGN KEY(team_number) REFERENCES team(team_number)
    -- Removed FOREIGN KEY(team_number) REFERENCES sqlite_sequence as it's specific to SQLite
);

CREATE TABLE IF NOT EXISTS room_schedule (
    room_id INT NOT NULL,
    timeslot VARCHAR(255) NOT NULL,
    team_number INT NOT NULL,
    PRIMARY KEY(room_id),
    FOREIGN KEY(room_id) REFERENCES room(room_id),
    FOREIGN KEY(team_number) REFERENCES team(team_number)
);

CREATE TABLE IF NOT EXISTS judge_room (
    user_id INT NOT NULL,
    room_id INT NOT NULL,
    PRIMARY KEY(user_id, room_id),
    FOREIGN KEY(room_id) REFERENCES room_schedule(room_id),
    FOREIGN KEY(user_id) REFERENCES user(user_id)
);

CREATE TABLE IF NOT EXISTS team_submission (
    team_number INT NOT NULL,
    github_link VARCHAR(255),
    video_link VARCHAR(255),
    timestamp VARCHAR(255),
    PRIMARY KEY(team_number),
    FOREIGN KEY(team_number) REFERENCES team(team_number)
);

CREATE TABLE IF NOT EXISTS peer_evaluation (
    peer_evaluation_id INT NOT NULL AUTO_INCREMENT,
    evaluator_id INT NOT NULL,
    subject_id INT NOT NULL,
    question_id INT NOT NULL,
    rating INT NOT NULL,
    PRIMARY KEY(peer_evaluation_id),
    FOREIGN KEY(subject_id) REFERENCES student_team(user_id),
    FOREIGN KEY(question_id) REFERENCES peer_evaluation_question(question_id),
    FOREIGN KEY(evaluator_id) REFERENCES student_team(user_id)
);

CREATE TABLE IF NOT EXISTS presentation (
    presentation_id INT NOT NULL AUTO_INCREMENT,
    judge_id INT NOT NULL,
    team_number INT NOT NULL,
    communication_score INT NOT NULL,
    communication_notes VARCHAR(255),
    demonstration_score INT NOT NULL,
    demonstration_notes VARCHAR(255),
    client_needs_score INT NOT NULL,
    client_needs_notes VARCHAR(255),
    awards VARCHAR(255),
    team_rank INT,
    PRIMARY KEY(presentation_id),
    FOREIGN KEY(team_number) REFERENCES team(team_number),
    FOREIGN KEY(judge_id) REFERENCES judge_room(user_id)
);


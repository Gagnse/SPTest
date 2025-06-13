-- CREATE DATABASE SPACELOGIC_ADMIN_DB;


-- Section BD administrative
-- ----------------------------TABLES---------------------------
-- Organizations table
CREATE TABLE organizations (
    id UUID DEFAULT (gen_random_uuid()) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ,
    super_admin_id UUID,
    UNIQUE(name)
);

-- Users table
CREATE TABLE users (
    id UUID DEFAULT (gen_random_uuid()) PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    organization_id UUID NOT NULL,
    role VARCHAR(50),
    department VARCHAR(100),
    location VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_active TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (organization_id) REFERENCES organizations(id),
    UNIQUE(email)
);

-- Add foreign key constraint for super_admin
ALTER TABLE organizations
    ADD CONSTRAINT fk_super_admin
    FOREIGN KEY (super_admin_id) REFERENCES users(id)
    ON UPDATE CASCADE;

-- Organization roles
CREATE TABLE organization_roles (
    id UUID DEFAULT (gen_random_uuid()) PRIMARY KEY,
    organization_id UUID NOT NULL,
    name VARCHAR(50) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (organization_id) REFERENCES organizations(id)
    ON DELETE CASCADE
);

-- Projects
CREATE TABLE projects (
    id UUID DEFAULT (gen_random_uuid()) PRIMARY KEY,
    organization_id UUID NOT NULL,
    project_number VARCHAR(50) NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    start_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    end_date TIMESTAMP,
    status VARCHAR(50) DEFAULT 'active',
    type VARCHAR(50),
    image_url VARCHAR(255) DEFAULT ('/static/images/project-placeholder.png'),
    FOREIGN KEY (organization_id) REFERENCES organizations(id),
    UNIQUE(project_number)
);

-- Create invitations table
CREATE TABLE organization_invitations (
    id UUID DEFAULT (gen_random_uuid()) PRIMARY KEY,
    organization_id UUID NOT NULL,
    email VARCHAR(255) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    role VARCHAR(50) NOT NULL,
    department VARCHAR(100),
    location VARCHAR(100),
    token VARCHAR(100) NOT NULL,
    invited_by UUID NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP NOT NULL,
    status VARCHAR(10) CHECK (status IN ('pending', 'accepted', 'expired', 'cancelled')) DEFAULT 'pending',
    FOREIGN KEY (organization_id) REFERENCES organizations(id) ON DELETE CASCADE,
    FOREIGN KEY (invited_by) REFERENCES users(id) ON DELETE CASCADE,
    UNIQUE (email, organization_id, status)
);

CREATE INDEX idx_token ON organization_invitations(token);

-- -----------------------------RELATIONS-----------------------------

-- User-organization relationship
CREATE TABLE organization_user (
    organizations_id UUID NOT NULL,
    users_id UUID NOT NULL,
    joined_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    PRIMARY KEY (organizations_id, users_id),
    FOREIGN KEY (organizations_id)
    REFERENCES organizations(id)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
    FOREIGN KEY (users_id)
    REFERENCES users(id)
    ON DELETE CASCADE
    ON UPDATE CASCADE
);

-- User-organization-role assignment
CREATE TABLE user_organisation_role (
    user_id UUID NOT NULL,
    organisation_id UUID NOT NULL,
    role_id UUID NOT NULL,
    assigned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, organisation_id, role_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (organisation_id) REFERENCES organizations(id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES organization_roles(id) ON DELETE CASCADE
);

-- Project user assignment
CREATE TABLE project_users (
    project_id UUID NOT NULL,
    user_id UUID NOT NULL,
    joined_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    role_id UUID NOT NULL,
    PRIMARY KEY (project_id, user_id),
    FOREIGN KEY (project_id) REFERENCES projects(id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES organization_roles(id) ON DELETE CASCADE
);
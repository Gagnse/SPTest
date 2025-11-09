-- Migration: Add Permission System and Soft Delete Support
-- Created: 2025-01-17
-- Description: Adds granular permission system with roles and email token management

-- =====================================================
-- 1. Add soft delete support to users table
-- =====================================================
ALTER TABLE users ADD COLUMN IF NOT EXISTS deleted_at TIMESTAMP NULL;

-- Create index for performance on soft delete queries
CREATE INDEX IF NOT EXISTS idx_users_deleted_at ON users(deleted_at) WHERE deleted_at IS NULL;

-- =====================================================
-- 2. Create permissions table
-- =====================================================
CREATE TABLE IF NOT EXISTS permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    resource VARCHAR(50) NOT NULL,
    action VARCHAR(50) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UNIQUE(resource, action)
);

-- Create index for permission lookups
CREATE INDEX IF NOT EXISTS idx_permissions_resource_action ON permissions(resource, action);

-- =====================================================
-- 3. Create role_permissions junction table
-- =====================================================
CREATE TABLE IF NOT EXISTS role_permissions (
    role_id UUID NOT NULL,
    permission_id UUID NOT NULL,
    granted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    PRIMARY KEY (role_id, permission_id),
    FOREIGN KEY (role_id) REFERENCES organization_roles(id) ON DELETE CASCADE,
    FOREIGN KEY (permission_id) REFERENCES permissions(id) ON DELETE CASCADE
);

-- Create indexes for role permission lookups
CREATE INDEX IF NOT EXISTS idx_role_permissions_role ON role_permissions(role_id);
CREATE INDEX IF NOT EXISTS idx_role_permissions_permission ON role_permissions(permission_id);

-- =====================================================
-- 4. Create email_tokens table
-- =====================================================
CREATE TABLE IF NOT EXISTS email_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NULL,
    email VARCHAR(255) NOT NULL,
    token VARCHAR(500) NOT NULL UNIQUE,
    token_type VARCHAR(50) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    used_at TIMESTAMP NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Create indexes for token lookups
CREATE INDEX IF NOT EXISTS idx_email_tokens_token ON email_tokens(token);
CREATE INDEX IF NOT EXISTS idx_email_tokens_email ON email_tokens(email);
CREATE INDEX IF NOT EXISTS idx_email_tokens_type ON email_tokens(token_type);
CREATE INDEX IF NOT EXISTS idx_email_tokens_expires ON email_tokens(expires_at);

-- =====================================================
-- 5. Seed default permissions
-- =====================================================
-- User management permissions
INSERT INTO permissions (name, resource, action, description) VALUES
('users.create', 'users', 'create', 'Create new users'),
('users.read', 'users', 'read', 'View user profiles and information'),
('users.update', 'users', 'update', 'Edit user information'),
('users.delete', 'users', 'delete', 'Soft delete users'),
('users.hardDelete', 'users', 'hardDelete', 'Permanently delete users (admin only)')
ON CONFLICT (resource, action) DO NOTHING;

-- Project management permissions
INSERT INTO permissions (name, resource, action, description) VALUES
('projects.create', 'projects', 'create', 'Create new projects'),
('projects.read', 'projects', 'read', 'View project details'),
('projects.update', 'projects', 'update', 'Edit project information'),
('projects.delete', 'projects', 'delete', 'Delete projects')
ON CONFLICT (resource, action) DO NOTHING;

-- Role management permissions
INSERT INTO permissions (name, resource, action, description) VALUES
('roles.create', 'roles', 'create', 'Create new roles'),
('roles.read', 'roles', 'read', 'View roles and permissions'),
('roles.update', 'roles', 'update', 'Edit role information'),
('roles.delete', 'roles', 'delete', 'Delete roles'),
('roles.manage', 'roles', 'manage', 'Full role and permission management')
ON CONFLICT (resource, action) DO NOTHING;

-- Invitation permissions
INSERT INTO permissions (name, resource, action, description) VALUES
('invitations.send', 'invitations', 'send', 'Send user invitations'),
('invitations.cancel', 'invitations', 'cancel', 'Cancel pending invitations'),
('invitations.view', 'invitations', 'view', 'View organization invitations')
ON CONFLICT (resource, action) DO NOTHING;

-- Organization management permissions
INSERT INTO permissions (name, resource, action, description) VALUES
('organization.manage', 'organization', 'manage', 'Manage organization settings'),
('organization.view', 'organization', 'view', 'View organization information')
ON CONFLICT (resource, action) DO NOTHING;

-- Admin permissions
INSERT INTO permissions (name, resource, action, description) VALUES
('admin.all', 'admin', 'all', 'Full administrative access to all resources')
ON CONFLICT (resource, action) DO NOTHING;

-- =====================================================
-- COMMENTS
-- =====================================================
COMMENT ON TABLE permissions IS 'System-wide permissions that can be assigned to roles';
COMMENT ON TABLE role_permissions IS 'Junction table linking roles to permissions';
COMMENT ON TABLE email_tokens IS 'Tokens for email verification, invitations, and password resets';
COMMENT ON COLUMN users.deleted_at IS 'Soft delete timestamp - NULL means user is active';
COMMENT ON COLUMN email_tokens.token_type IS 'Type of token: invitation, password_reset, email_verification';
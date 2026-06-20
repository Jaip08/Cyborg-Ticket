CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE SEQUENCE ticket_number_seq START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE TABLE categories (
        id uuid NOT NULL,
        name character varying(100) NOT NULL,
        description character varying(500),
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone,
        CONSTRAINT pk_categories PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE TABLE roles (
        id uuid NOT NULL,
        name character varying(50) NOT NULL,
        description character varying(200),
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone,
        CONSTRAINT pk_roles PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE TABLE users (
        id uuid NOT NULL,
        full_name character varying(120) NOT NULL,
        email character varying(256) NOT NULL,
        password_hash text NOT NULL,
        role_id uuid NOT NULL,
        is_active boolean NOT NULL,
        last_login_at timestamp without time zone,
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone,
        CONSTRAINT pk_users PRIMARY KEY (id),
        CONSTRAINT fk_users_roles_role_id FOREIGN KEY (role_id) REFERENCES roles (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE TABLE password_reset_tokens (
        id uuid NOT NULL,
        user_id uuid NOT NULL,
        token_hash character varying(128) NOT NULL,
        expires_at timestamp without time zone NOT NULL,
        used_at timestamp without time zone,
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone,
        CONSTRAINT pk_password_reset_tokens PRIMARY KEY (id),
        CONSTRAINT fk_password_reset_tokens_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE TABLE tickets (
        id uuid NOT NULL,
        ticket_number character varying(20) NOT NULL DEFAULT ('TKT-' || lpad(nextval('ticket_number_seq')::text, 5, '0')),
        title character varying(200) NOT NULL,
        description text NOT NULL,
        status character varying(20) NOT NULL,
        priority character varying(20) NOT NULL,
        category_id uuid NOT NULL,
        created_by_id uuid NOT NULL,
        assigned_to_id uuid,
        due_date timestamp without time zone,
        resolved_at timestamp without time zone,
        closed_at timestamp without time zone,
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone,
        CONSTRAINT pk_tickets PRIMARY KEY (id),
        CONSTRAINT fk_tickets_categories_category_id FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE RESTRICT,
        CONSTRAINT fk_tickets_users_assigned_to_id FOREIGN KEY (assigned_to_id) REFERENCES users (id) ON DELETE SET NULL,
        CONSTRAINT fk_tickets_users_created_by_id FOREIGN KEY (created_by_id) REFERENCES users (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE TABLE activity_logs (
        id uuid NOT NULL,
        ticket_id uuid NOT NULL,
        user_id uuid NOT NULL,
        action character varying(50) NOT NULL,
        description character varying(500) NOT NULL,
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone,
        CONSTRAINT pk_activity_logs PRIMARY KEY (id),
        CONSTRAINT fk_activity_logs_tickets_ticket_id FOREIGN KEY (ticket_id) REFERENCES tickets (id) ON DELETE CASCADE,
        CONSTRAINT fk_activity_logs_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE TABLE ticket_attachments (
        id uuid NOT NULL,
        ticket_id uuid NOT NULL,
        file_name character varying(255) NOT NULL,
        stored_name character varying(255) NOT NULL,
        content_type character varying(150) NOT NULL,
        file_size bigint NOT NULL,
        uploaded_by_id uuid NOT NULL,
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone,
        CONSTRAINT pk_ticket_attachments PRIMARY KEY (id),
        CONSTRAINT fk_ticket_attachments_tickets_ticket_id FOREIGN KEY (ticket_id) REFERENCES tickets (id) ON DELETE CASCADE,
        CONSTRAINT fk_ticket_attachments_users_uploaded_by_id FOREIGN KEY (uploaded_by_id) REFERENCES users (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE TABLE ticket_comments (
        id uuid NOT NULL,
        ticket_id uuid NOT NULL,
        author_id uuid NOT NULL,
        content character varying(4000) NOT NULL,
        is_internal boolean NOT NULL,
        created_at timestamp without time zone NOT NULL,
        updated_at timestamp without time zone,
        CONSTRAINT pk_ticket_comments PRIMARY KEY (id),
        CONSTRAINT fk_ticket_comments_tickets_ticket_id FOREIGN KEY (ticket_id) REFERENCES tickets (id) ON DELETE CASCADE,
        CONSTRAINT fk_ticket_comments_users_author_id FOREIGN KEY (author_id) REFERENCES users (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_activity_logs_ticket_id ON activity_logs (ticket_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_activity_logs_user_id ON activity_logs (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE UNIQUE INDEX ix_categories_name ON categories (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_password_reset_tokens_token_hash ON password_reset_tokens (token_hash);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_password_reset_tokens_user_id ON password_reset_tokens (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE UNIQUE INDEX ix_roles_name ON roles (name);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_ticket_attachments_ticket_id ON ticket_attachments (ticket_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_ticket_attachments_uploaded_by_id ON ticket_attachments (uploaded_by_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_ticket_comments_author_id ON ticket_comments (author_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_ticket_comments_ticket_id ON ticket_comments (ticket_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_tickets_assigned_to_id ON tickets (assigned_to_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_tickets_category_id ON tickets (category_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_tickets_created_at ON tickets (created_at);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_tickets_created_by_id ON tickets (created_by_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_tickets_priority ON tickets (priority);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_tickets_status ON tickets (status);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE UNIQUE INDEX ix_tickets_ticket_number ON tickets (ticket_number);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE UNIQUE INDEX ix_users_email ON users (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    CREATE INDEX ix_users_role_id ON users (role_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260620045757_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20260620045757_InitialCreate', '8.0.8');
    END IF;
END $EF$;
COMMIT;


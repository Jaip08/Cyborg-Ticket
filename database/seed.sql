-- Seed data for the Ticket Management System.
-- Safe to run on a fresh database created from schema.sql. Re-running won't duplicate
-- the lookup/account rows; sample tickets are only inserted when the table is empty.
--
-- Demo accounts (password in brackets):
--   admin@ticket.local     (Admin@123)     - Admin
--   manager@ticket.local   (Manager@123)   - Manager
--   employee@ticket.local  (Employee@123)  - Employee
--   priya@ticket.local     (Employee@123)  - Employee

begin;

insert into roles (id, name, description, created_at)
values
    (gen_random_uuid(), 'Admin',    'Full administrative access',                 now()),
    (gen_random_uuid(), 'Manager',  'Manages tickets, assignments and reporting', now()),
    (gen_random_uuid(), 'Employee', 'Raises and works on tickets',                now())
on conflict (name) do nothing;

insert into categories (id, name, description, created_at)
values
    (gen_random_uuid(), 'Hardware',         'Laptops, peripherals and physical equipment', now()),
    (gen_random_uuid(), 'Software',         'Applications, licences and installs',          now()),
    (gen_random_uuid(), 'Network',          'Connectivity, VPN and Wi-Fi issues',          now()),
    (gen_random_uuid(), 'Account & Access', 'Logins, permissions and provisioning',        now()),
    (gen_random_uuid(), 'Billing',          'Invoices, subscriptions and payments',        now()),
    (gen_random_uuid(), 'General',          'Anything that does not fit elsewhere',        now())
on conflict (name) do nothing;

insert into users (id, full_name, email, password_hash, role_id, is_active, created_at)
values
    (gen_random_uuid(), 'Site Administrator', 'admin@ticket.local',
        '$2a$11$nSJjPOOUfK/Ni55E07uMOO24Zzw5c2.q53OxYDnXAWBRkeryA/fXm',
        (select id from roles where name = 'Admin'), true, now()),
    (gen_random_uuid(), 'Maria Chen', 'manager@ticket.local',
        '$2a$11$IW9UR/feBptAci/PJdWsAe.1BgNYSQ3cE0ASlt1Bhuli7ZrAzbolW',
        (select id from roles where name = 'Manager'), true, now()),
    (gen_random_uuid(), 'Daniel Okafor', 'employee@ticket.local',
        '$2a$11$vE1.hJlulttzJ40dQoAA0uKknGn.1es9muRKpXxLKlg9UgOx/VBoW',
        (select id from roles where name = 'Employee'), true, now()),
    (gen_random_uuid(), 'Priya Nair', 'priya@ticket.local',
        '$2a$11$vE1.hJlulttzJ40dQoAA0uKknGn.1es9muRKpXxLKlg9UgOx/VBoW',
        (select id from roles where name = 'Employee'), true, now())
on conflict (email) do nothing;

do $$
declare
    cat_hardware uuid := (select id from categories where name = 'Hardware');
    cat_software uuid := (select id from categories where name = 'Software');
    cat_network  uuid := (select id from categories where name = 'Network');
    cat_access   uuid := (select id from categories where name = 'Account & Access');
    cat_billing  uuid := (select id from categories where name = 'Billing');
    cat_general  uuid := (select id from categories where name = 'General');
    u_manager  uuid := (select id from users where email = 'manager@ticket.local');
    u_daniel   uuid := (select id from users where email = 'employee@ticket.local');
    u_priya    uuid := (select id from users where email = 'priya@ticket.local');
    vpn_id     uuid;
    finance_id uuid;
begin
    if exists (select 1 from tickets) then
        return;
    end if;

    insert into tickets (id, ticket_number, title, description, status, priority, category_id, created_by_id, assigned_to_id, due_date, resolved_at, closed_at, created_at)
    values
        (gen_random_uuid(), 'TKT-00001', 'Laptop won''t power on after update',
            'Since the latest Windows update my laptop shows a black screen on boot.',
            'InProgress', 'High', cat_hardware, u_daniel, u_priya, now() + interval '1 day', null, null, now() - interval '120 days'),
        (gen_random_uuid(), 'TKT-00002', 'VPN drops every few minutes',
            'The corporate VPN disconnects roughly every five minutes.',
            'Open', 'Critical', cat_network, u_manager, u_daniel, now() - interval '2 days', null, null, now() - interval '85 days'),
        (gen_random_uuid(), 'TKT-00003', 'Request: Adobe Photoshop licence',
            'I need a Photoshop licence for the new marketing campaign assets.',
            'OnHold', 'Low', cat_software, u_priya, null, null, null, null, now() - interval '78 days'),
        (gen_random_uuid(), 'TKT-00004', 'Cannot access shared finance drive',
            'Permission denied when opening the finance shared folder.',
            'Resolved', 'Medium', cat_access, u_daniel, u_manager, null, now() - interval '55 days', null, now() - interval '57 days'),
        (gen_random_uuid(), 'TKT-00005', 'Duplicate charge on March invoice',
            'We were billed twice for the March subscription.',
            'Closed', 'Medium', cat_billing, u_priya, u_daniel, null, now() - interval '48 days', now() - interval '47 days', now() - interval '51 days'),
        (gen_random_uuid(), 'TKT-00006', 'New starter onboarding setup',
            'Please prepare accounts, laptop and access for our new analyst.',
            'InProgress', 'High', cat_access, u_manager, u_priya, now() + interval '3 days', null, null, now() - interval '26 days'),
        (gen_random_uuid(), 'TKT-00007', 'Printer on 3rd floor jamming',
            'The shared printer keeps jamming on double-sided prints.',
            'Open', 'Low', cat_hardware, u_daniel, null, null, null, null, now() - interval '12 days'),
        (gen_random_uuid(), 'TKT-00008', 'Email signature not applying',
            'The company email signature is not added to outgoing mail.',
            'Open', 'Medium', cat_software, u_priya, u_daniel, null, null, null, now() - interval '3 days');

    perform setval('ticket_number_seq', 8);

    select id into vpn_id from tickets where ticket_number = 'TKT-00002';
    select id into finance_id from tickets where ticket_number = 'TKT-00004';

    insert into ticket_comments (id, ticket_id, author_id, content, is_internal, created_at)
    values
        (gen_random_uuid(), vpn_id, u_daniel, 'Reproduced on two machines, so it looks network-side.', false, now() - interval '84 days'),
        (gen_random_uuid(), vpn_id, u_manager, 'Escalating to the network vendor. Flagging as critical.', true, now() - interval '84 days'),
        (gen_random_uuid(), finance_id, u_manager, 'Re-added you to the finance group. Can you confirm access?', false, now() - interval '56 days'),
        (gen_random_uuid(), finance_id, u_daniel, 'Confirmed, I can open the folder again. Thanks!', false, now() - interval '55 days');

    insert into activity_logs (id, ticket_id, user_id, action, description, created_at)
    values
        (gen_random_uuid(), vpn_id, u_manager, 'Created', 'Opened ticket at Critical priority.', now() - interval '85 days'),
        (gen_random_uuid(), vpn_id, u_manager, 'Assigned', 'Assigned to Daniel Okafor.', now() - interval '85 days'),
        (gen_random_uuid(), finance_id, u_daniel, 'Created', 'Opened ticket at Medium priority.', now() - interval '57 days'),
        (gen_random_uuid(), finance_id, u_manager, 'StatusChanged', 'Status changed from Open to Resolved.', now() - interval '55 days');
end $$;

commit;

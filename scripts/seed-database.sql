BEGIN;

DELETE FROM schedules
WHERE employee_registration IN (
    'SEED-001',
    'SEED-002',
    'SEED-003',
    'SEED-004',
    'SEED-005'
);

INSERT INTO schedules (
    employee_name,
    employee_registration,
    department,
    shift_name,
    start_time,
    end_time,
    work_date,
    status,
    notes,
    created_at,
    updated_at
)
VALUES
(
    'Ana Martins',
    'SEED-001',
    'Atendimento',
    'Manha',
    '08:00:00',
    '12:00:00',
    CURRENT_DATE,
    'Scheduled',
    'Seed inicial para teste da listagem.',
    NOW(),
    NULL
),
(
    'Bruno Costa',
    'SEED-002',
    'Operacoes',
    'Tarde',
    '13:00:00',
    '17:00:00',
    CURRENT_DATE,
    'Scheduled',
    NULL,
    NOW(),
    NULL
),
(
    'Carla Souza',
    'SEED-003',
    'Financeiro',
    'Integral',
    '09:00:00',
    '18:00:00',
    CURRENT_DATE + 1,
    'Completed',
    'Registro de exemplo concluido.',
    NOW(),
    NULL
),
(
    'Diego Lima',
    'SEED-004',
    'Suporte',
    'Noite',
    '18:00:00',
    '22:00:00',
    CURRENT_DATE + 1,
    'Cancelled',
    'Escala cancelada para teste de status.',
    NOW(),
    NULL
),
(
    'Elisa Rocha',
    'SEED-005',
    'RH',
    'Manha',
    '08:30:00',
    '12:30:00',
    CURRENT_DATE + 2,
    'Absent',
    'Exemplo de ausencia.',
    NOW(),
    NULL
);

COMMIT;

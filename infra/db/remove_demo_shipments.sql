-- Remove demo shipments seeded in earlier builds.
-- Adjust tracking numbers if you have already created real shipments with these identifiers.
DELETE FROM shipment_status_histories
WHERE shipment_id IN (
    SELECT id FROM shipments WHERE tracking_number IN (
        'VTX-202411-0001',
        'VTX-202411-0002',
        'VTX-202411-0003',
        'VTX-202411-0004',
        'VTX-202411-0005'
    )
);

DELETE FROM shipments
WHERE tracking_number IN (
    'VTX-202411-0001',
    'VTX-202411-0002',
    'VTX-202411-0003',
    'VTX-202411-0004',
    'VTX-202411-0005'
);

CREATE PROCEDURE
    usp_events_add(arrived timestamp without time zone, jsonevent json, INOUT out_id bigint)
 LANGUAGE plpgsql
AS $$
DECLARE
    _arrived alias      FOR arrived;
    _jsonevent alias    FOR jsonevent;
    
    _core_uuid          uuid;
    _switch_id          bigint;
    _event_name         text;
    _event_date         timestamp;
BEGIN
    _arrived = COALESCE(arrived,now()::timestamp);

    _core_uuid = jsonevent->>'Core-UUID';
    _event_name = jsonevent->>'Event-Name';
    _event_date = to_timestamp(((jsonevent ->> 'Event-Date-Timestamp'::text)::bigint)::double precision 
        / 1000000::double precision);

    INSERT INTO switches(
        core_uuid
        , last_event
    ) VALUES (
        _core_uuid
        , _event_date
    )
    ON CONFLICT ON CONSTRAINT switches_un_core_uuid
    DO
        UPDATE
        SET last_event = _event_date
        WHERE switches.core_uuid = _core_uuid
    RETURNING id INTO _switch_id;

    IF _event_name IN ('HEARTBEAT') THEN

        INSERT INTO events_heartbeat_last(
            switch_id
            , event_jsonb
        ) VALUES (
             _switch_id
            , jsonevent
        )
        ON CONFLICT ON CONSTRAINT events_heartbeat_last_un_switch_id
        DO
            UPDATE
            SET event_jsonb = jsonevent
            WHERE events_heartbeat_last.switch_id = _switch_id
        RETURNING id INTO out_id;

    ELSE

        INSERT INTO events(
            switch_id
            , event_jsonb
        ) VALUES (
             _switch_id
            , jsonevent
        ) RETURNING id INTO out_id;
    
    END IF;

COMMIT;
END;
$$
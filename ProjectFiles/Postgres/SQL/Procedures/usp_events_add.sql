CREATE PROCEDURE
    usp_events_add(arrived timestamp, jsonevent json, INOUT out_id bigint)
AS $$
DECLARE
    _arrived alias for arrived;
    _jsonevent alias for jsonevent;
BEGIN

    _arrived = COALESCE (arrived, now()::timestamp);
    
    INSERT INTO events(
        arrived,
        jsonevent
    ) VALUES (
        _arrived,
        _jsonevent
    ) RETURNING ID INTO out_id;

END;
$$ LANGUAGE plpgsql
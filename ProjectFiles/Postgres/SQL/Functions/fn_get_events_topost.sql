CREATE FUNCTION
    fn_get_events_topost(url TEXT)
RETURNS TABLE (event_id bigint, event_jsonb jsonb)
LANGUAGE plpgsql
AS $$
DECLARE
    _url alias FOR url;
    _handler_id int;
BEGIN
    
    SELECT
        id
    FROM http_post_handlers h
    INTO _handler_id
    WHERE h.url = _url;

    RETURN query
    SELECT
        e.id
        , e.event_jsonb
    FROM events e
    WHERE e.id NOT IN (
        SELECT
            esent.event_id
        FROM http_post_statuses esent
        JOIN http_post_handlers h
        ON esent.handler_id IN(h.id)
        JOIN events e2
        ON esent.event_id  IN(e2.id)
        WHERE h.id IN(COALESCE(_handler_id,-1))
    )
    AND e.is_valid
    ORDER BY e.e_event_date;

END;
$$
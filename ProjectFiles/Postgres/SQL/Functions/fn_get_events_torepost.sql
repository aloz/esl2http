CREATE FUNCTION
    fn_get_events_torepost(url TEXT)
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
    JOIN http_post_statuses p
    ON p.event_id IN(e.id)
    WHERE p.need_resend
    AND p.handler_id = COALESCE(_handler_id,'-1')
    AND e.is_valid
    ORDER BY e.e_event_date;

END;
$$
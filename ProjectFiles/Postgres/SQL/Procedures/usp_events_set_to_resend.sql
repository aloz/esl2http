CREATE PROCEDURE
    usp_events_set_to_resend(url TEXT, INOUT id int)
LANGUAGE plpgsql
AS
$$
DECLARE
    _url alias FOR url;
    _handler_id alias FOR id;
BEGIN
    
    SELECT
        h.id
    FROM http_post_handlers h
    INTO _handler_id
    WHERE h.url = _url;

    UPDATE http_post_statuses
    SET need_resend = TRUE
    WHERE handler_id = COALESCE (_handler_id, -1)
    AND NOT is_success;
    
END;
$$
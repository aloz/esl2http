CREATE PROCEDURE
    usp_is_resend_available(url TEXT, INOUT out_is_available bool)
LANGUAGE plpgsql
AS
$$
DECLARE
    _url        alias FOR url;
    _handler_id int;
    _rcount     bigint;
BEGIN
    
    SELECT
        id
    FROM http_post_handlers h
    INTO _handler_id
    WHERE h.url = _url;

    IF _handler_id IS NOT NULL THEN
        
        SELECT
            count(*)
        FROM http_post_statuses
        INTO _rcount
        WHERE handler_id = _handler_id
        AND NOT is_success;
        
        IF _rcount > 0
        THEN out_is_available = TRUE;
        ELSE out_is_available = FALSE;
        END IF;
    
    END IF;
    
END;
$$
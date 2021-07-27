-- This function is unused
CREATE FUNCTION
    fn_is_http_statuscode_success(statuscode int, null2null bool)
    RETURNS bool
LANGUAGE plpgsql
AS
$$
DECLARE
    _result bool;
BEGIN
    
    IF null2null THEN
        IF statuscode IS NULL THEN
            _result = NULL;
        ELSE
            _result = last_statuscode >= 200 AND last_statuscode < 300;
        END IF;
    ELSE
        _result = COALESCE(last_statuscode, '-1'::integer) >= 200 AND COALESCE(last_statuscode, '-1'::integer) < 300;
    END IF;

    RETURN _result;
    
END;
$$
CREATE FUNCTION
    fn_get_http_handlers_torepost()
RETURNS TABLE (url TEXT)
LANGUAGE plpgsql
AS
$$
BEGIN
    
    RETURN query
    SELECT
        h.url
    FROM http_post_handlers h
    WHERE disabled IS NULL
    AND id IN (
        SELECT
            handler_id
        FROM
            http_post_statuses
        WHERE
            need_resend
    )
    ORDER BY id;

END;
$$
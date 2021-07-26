CREATE OR REPLACE FUNCTION
    fn_get_http_handlers()
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
    AND id NOT IN (
        SELECT
            handler_id
        FROM
            http_post_statuses
        WHERE
            NOT is_success
    )
    ORDER BY id;

END;
$$
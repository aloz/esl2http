CREATE FUNCTION
    fn_get_http_handlers()
RETURNS TABLE (url TEXT)
LANGUAGE plpgsql
AS
$$
BEGIN
    RETURN query
    SELECT h.url
    FROM http_post_handlers h
    WHERE disabled IS NULL
    ORDER BY h.id;
END;
$$ 

CREATE FUNCTION
    fn_get_config()
RETURNS SETOF config
LANGUAGE plpgsql
AS
$$
BEGIN
    RETURN query 
    SELECT * FROM config
    FETCH FIRST ROW ONLY;
END;
$$
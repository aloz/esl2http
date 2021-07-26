CREATE PROCEDURE
    usp_events_set_as_posted(event_id bigint, url TEXT, statuscode int, reason_phrase text, INOUT out_id bigint)
LANGUAGE plpgsql
AS $$
DECLARE
    _event_id       alias FOR event_id;
    _url            alias FOR url;
    _statuscode     alias FOR statuscode;
    _reason_phrase  alias FOR reason_phrase;

    _handler_id     int;
    _utcnow         timestamp;
BEGIN
    
    _utcnow = now()::timestamp;
    
    INSERT INTO http_post_handlers (
        url
        , last_statuscode
    ) VALUES (
        _url
        , _statuscode
    )
    ON CONFLICT ON CONSTRAINT http_post_handlers_un_url
    DO
        UPDATE
        SET
            last_posted         = _utcnow
            , last_statuscode   = _statuscode
        WHERE http_post_handlers.url = _url
    RETURNING id INTO _handler_id;

    INSERT INTO http_post_statuses (
        event_id
        , handler_id
        , posted
        , statuscode
        , reason_phrase
    ) VALUES (
        _event_id
        , _handler_id
        , _utcnow
        , _statuscode
        , _reason_phrase
    )
    ON CONFLICT ON CONSTRAINT http_post_statuses_un_event_id_handler_id
    DO
        UPDATE
        SET
            posted          = _utcnow
            , statuscode    = _statuscode
            , reason_phrase = _reason_phrase
        WHERE http_post_statuses.event_id = _event_id AND http_post_statuses.handler_id = _handler_id
    RETURNING id INTO out_id;

COMMIT;
END;
$$

-- This is an example how to apply to send events to the failed http handler

DO
LANGUAGE plpgsql
$$
DECLARE
    _handler_id int;
BEGIN
    CALL usp_events_set_to_resend('https://ptsv2.com/t/iev4l-1627303429/post', _handler_id);
    RAISE NOTICE '_handler_id: %', _handler_id;
END;
$$
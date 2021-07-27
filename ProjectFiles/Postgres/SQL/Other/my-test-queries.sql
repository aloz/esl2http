-- I'm using it to test the behavior

SELECT * FROM events ORDER BY id DESC;
SELECT * FROM events_heartbeat_last ORDER BY id DESC;
SELECT * FROM switches;

DELETE FROM http_post_statuses;

SELECT * FROM http_post_statuses ORDER BY id DESC;
SELECT * FROM http_post_handlers ORDER BY id DESC;

SELECT * FROM fn_get_http_handlers();
SELECT * FROM fn_get_events_topost();

SELECT *  FROM fn_get_config();

DELETE FROM config;

INSERT INTO config(timeout_s_http) VALUES (100);

SELECT * FROM fn_get_events_topost('https://ptsv2.com/t/1fnkf-1627122772/post');

INSERT INTO http_post_handlers(url) VALUES ('https://ptsv2.com/t/g9qtp-1627257351qwerty12333337/post');
INSERT INTO http_post_handlers(url) VALUES ('https://ptsv2.com/t/iev4l-1627303429/post');

DELETE FROM http_post_handlers WHERE url =  'https://ptsv2.com/t/g9qtp-1627257351qwerty/post';
DELETE FROM http_post_handlers WHERE url =  'https://ptsv2.com/t/lxlxm-1627287724/post';

DELETE FROM events_heartbeat_last;
DELETE FROM events;
DELETE FROM switches;

SELECT * FROM http_post_handlers;
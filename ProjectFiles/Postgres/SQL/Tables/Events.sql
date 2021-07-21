CREATE TABLE events (
	id bigserial NOT NULL,
	arrived timestamp NOT NULL DEFAULT now()::timestamp,
	jsonevent json NOT NULL,
	PRIMARY KEY (ID)
);
CREATE UNIQUE INDEX events_id_idx ON Events USING btree (id); 
CREATE INDEX events_arrived_idx ON Events USING btree (arrived);
CREATE TABLE Events (
	ID int8range NOT NULL,
	Arrived timestamp NOT NULL DEFAULT now(),
	JsonEvent json NOT NULL
);
CREATE UNIQUE INDEX IX_Events_ID ON Events USING btree (ID); 
CREATE INDEX IX_Events_Arrived ON Events USING btree (Arrived);
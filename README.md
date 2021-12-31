# Debezium Outbox Sample

Tiny sample application showing how we could use Debezium to publish our outbox messages

**NOTE** It should go without saying that this **is not production ready**.

## About the solution

The solution is comprised of 3 projects

- Producer - Application that stores a collection of messages in the outbox table. Doesn't do anything else, as Debezium is in charge of doing the actual publishing.
- Consumer - Subscribes to events, logging every time a new one arrives, just to show that the messages put in the outbox are being published to Kafka.
- Events - Contains the events that are used in the solution. Also contains interfaces (and implementations) for publishing and subscribing to events.

In the root of the solution, there's a Docker Compose file to spin up the necessary dependencies, which are PostgreSQL, Kafka and Seq.

Using JSON serialization for the events, good enough for demo purposes. For production scenarios, something like ProtoBuf or Avro are probably better options.
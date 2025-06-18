# Endpoints de Creación de Transacciones

## Nuevos Endpoints Agregados

### 1. Crear Transacción y Nuevo Bloque

**POST** `/api/block/create-transaction`

Este endpoint crea una nueva transacción y la agrega a un nuevo bloque, persistiendo todos los cambios en la base de datos Neo4j.

#### Request Body:

```json
{
  "fromAddress": "direccion_origen",
  "toAddress": "direccion_destino",
  "amount": 100.5,
  "version": 1,
  "txSize": 250,
  "prevBlockHash": "hash_bloque_anterior" // Opcional
}
```

#### Response:

```json
{
  "message": "Transacción creada y agregada a un nuevo bloque exitosamente",
  "blockHash": "nuevo_hash_del_bloque",
  "transactionId": "id_de_la_transaccion",
  "timestamp": 1703123456,
  "amount": 100.5,
  "toAddress": "direccion_destino"
}
```

### 2. Agregar Transacción a Bloque Existente

**POST** `/api/block/add-transaction-to-block`

Este endpoint agrega una nueva transacción a un bloque existente.

#### Request Body:

```json
{
  "fromAddress": "direccion_origen",
  "toAddress": "direccion_destino",
  "amount": 50.25,
  "version": 1,
  "txSize": 200,
  "prevBlockHash": "hash_del_bloque_existente" // Requerido
}
```

#### Response:

```json
{
  "message": "Transacción agregada al bloque existente exitosamente",
  "blockHash": "hash_del_bloque",
  "transactionId": "id_de_la_transaccion",
  "amount": 50.25,
  "toAddress": "direccion_destino"
}
```

## Estructura de Datos en Neo4j

Los endpoints crean la siguiente estructura en la base de datos:

```
(block) -[:inc]-> (tx) -[:coinbase]-> (coinbase)
                |
                -[:out]-> (output)
```

### Nodos Creados:

- **block**: Contiene hash, size, timestamp, prevblock
- **tx**: Contiene txid, version, locktime, size, weight, segwit
- **coinbase**: Contiene index, value, addresses
- **output**: Contiene index, value, addresses

### Relaciones:

- `(block)-[:inc]->(tx)`: El bloque incluye la transacción
- `(tx)-[:coinbase]->(coinbase)`: La transacción tiene una entrada coinbase
- `(tx)-[:out]->(output)`: La transacción tiene un output
- `(prevBlock)-[:next]->(newBlock)`: Relación entre bloques consecutivos

## Ejemplo de Uso

### Crear una nueva transacción con nuevo bloque:

```bash
curl -X POST "https://localhost:7001/api/block/create-transaction" \
     -H "Content-Type: application/json" \
     -d '{
         "fromAddress": "1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa",
         "toAddress": "3J98t1WpEZ73CNmQviecrnyiWrnqRhWNLy",
         "amount": 0.001,
         "version": 1,
         "txSize": 250
     }'
```

### Agregar transacción a bloque existente:

```bash
curl -X POST "https://localhost:7001/api/block/add-transaction-to-block" \
     -H "Content-Type: application/json" \
     -d '{
         "fromAddress": "1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa",
         "toAddress": "3J98t1WpEZ73CNmQviecrnyiWrnqRhWNLy",
         "amount": 0.001,
         "version": 1,
         "txSize": 250,
         "prevBlockHash": "000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f"
     }'
```

## Características

- ✅ **Persistencia completa**: Todos los cambios se guardan en Neo4j
- ✅ **Generación automática**: Hash de bloque y ID de transacción se generan automáticamente
- ✅ **Validación**: Validación de datos de entrada
- ✅ **Manejo de errores**: Respuestas de error detalladas
- ✅ **Relaciones**: Creación automática de relaciones entre nodos
- ✅ **Timestamp**: Timestamp automático en formato Unix

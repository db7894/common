console.log('loading cloud search update function');

/**
 * Given an updated record, convert that to a cloudsearch
 * indexable document. It should be noted that there are three
 * possible inputs:
 *
 * - (null, new) - create from DDB means add to search
 * - (old, null) - delete from DDB means delete from search
 * - (old,  new) - update from DDB means add to search
 *
 * @param record The record to build a cloudsearch document with.
 * @returns The cloudsearch document for that record.
 */
function build_document(record) {
    console.log("applying (%s): %s", record.eventID, record.eventName);
    
    var oldest = record.dynamodb.OldImage,
        latest = record.dynamodb.NewImage,
        update = (latest || oldest);

    for (var key in update) {
        if (update.hasOwnProperty(key) {
            var type = Object.keys(update[key])[0];
            update[key] = update[key][type];
        }
    }

    var result = {
        'type' : (latest) ? "add" : "delete",
        'id'   : update['id']
    };

    if (latest) {
        result['fields'] = update;
    }

    return result;
}

/**
 * The main handler function that is called whenever an update
 * from the DynamoDB streams API alerts the Lambda function.
 */
exports.handler = function(event, context) {
    var search = new AWS.CloudSearchDomain({ endpoint: "endpoint" });
    var params = {
        contentType: 'application/json',
        documents: event.Records.map(build_document)
    };

    search.uploadDocuments(params, function(error, response) {
        if (error) {
            console.log(error, error.stack);
        } else {
            console.log(response):
        }
    });
    context.succeed("Successfully processed " + event.Records.length + " records.");
};

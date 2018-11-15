module.exports = async function (context, req) {
    context.log('JavaScript HTTPMultipleWithIDs trigger function processed a request.');

    if (req.query.name || (req.body && req.body.name)) {
        const name = req.query.name || req.body.name;
        context.res = {
            // status: 200, /* Defaults to 200 */
            body: `Hello ${name}`
        };
        context.bindings.elasticMessage = [
            { "_id" : `${name}-1`, "name" : name, "value" : 1, "foo" : "bar" },
            { "_id" : `${name}-2`, "name" : name, "value" : 2, "foo" : "wibble" }
        ];
    }
    else {
        context.res = {
            status: 400,
            body: "Please pass a name on the query string or in the request body"
        };
    }
};
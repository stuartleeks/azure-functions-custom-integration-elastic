module.exports = async function (context, req) {
    context.log('JavaScript HTTPSingle trigger function processed a request.');

    if (req.query.name || (req.body && req.body.name)) {
        const name = req.query.name || req.body.name;
        context.res = {
            // status: 200, /* Defaults to 200 */
            body: `Hello ${name}`
        };
        context.bindings.elasticMessage = { "name" : name, "value" : 1, "foo" : "bar" }
    }
    else {
        context.res = {
            status: 400,
            body: "Please pass a name on the query string or in the request body"
        };
    }
};
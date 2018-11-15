module.exports = async function (context, documents) {
    if (!!documents){
        context.bindings.elasticMessages = [];
        for (let index = 0; index < documents.length; index++) {
            const element = documents[index];
            context.bindings.elasticMessages.push({
                '_id': element.name,  
                'name' : element.name, 
                'value' : element.value
            });
        }
    }
}

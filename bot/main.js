const axios = require("axios")

const {SSMClient, GetParameterCommand} = require("@aws-sdk/client-ssm")
const { S3 } = require("@aws-sdk/client-s3")
const ssm = new SSMClient({region: process.env.AWS_REGION})
const s3client = new S3({region: process.env.AWS_REGION});

async function streamToString (stream) {
    return await new Promise((resolve, reject) => {
        const chunks = [];
        stream.on('data', (chunk) => chunks.push(chunk));
        stream.on('error', reject);
        stream.on('end', () => resolve(Buffer.concat(chunks).toString('utf-8')));
    });
}

exports.handler = async (event, context) => {
    console.log("get param")
    const token = await ssm.send(new GetParameterCommand({
        Name: process.env.TOKEN_SSM_PARAM_NAME,
        WithDecryption: true
    }));
    const pasteEEToken = await ssm.send(new GetParameterCommand({
        Name: process.env.PASTEEE_TOKEN_SSM_PARAM_NAME,
        WithDecryption: true
    }));
    const {bucketName, objectKey} = {
        bucketName: event.Records[0].s3.bucket.name,
        objectKey: event.Records[0].s3.object.key
    }
    const res = await s3client.getObject({Bucket: bucketName, Key: objectKey})
    const test = await streamToString(res.Body)

    const data = JSON.parse(test)

    let numberOfJobs = 0;
    data.forEach(e => numberOfJobs += e.Jobs.length)

    // register data
    const postConfig = {
        method: 'post',
        url: 'https://api.paste.ee/v1/pastes',
        headers: {
            'Content-Type': 'application/json',
            'X-Auth-Token': pasteEEToken.Parameter.Value
        },
        data : JSON.stringify({
            "description": "test",
            "sections": [
                {
                    "name": `${numberOfJobs} nouveaux jobs de ${data.length} entreprises aujourd'hui !`,
                    "syntax": "autodetect",
                    "contents": data.map(e =>
                        e.CompanyName + "\n" + e.Jobs.map(f =>
                            `${f.JobTitle}`
                        ).join("\n")

                    ).join("\n\n")

                }
            ]
        })
    };

    const pasteData = (await axios(postConfig)).data;


    try {
        const axiosResponse = await axios.post(`https://discord.com/api/webhooks/849767517979148298/${token.Parameter.Value}`, {
            "content": `Voici les jobs du jour : [ICI](${pasteData.link})`           ,
                    })
    } catch (e) {
        console.warn(e)
    }
    return
}

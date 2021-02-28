const fs = require("fs")

const body = fs.readFile("./linkedin_aggregated.json", {encoding: 'utf-8'},(err, data) => {
    if(err){
        console.log(err)
    }
    Object.keys = function (obj) {
        var keys = [],
            k;
        for (k in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, k)) {
                keys.push(k);
            }
        }
        return keys;
    };
    const jobs = JSON.parse(data)
    const result = jobs.map(e => {
        return {
            company: e.Company,
            jobsCount: Object.keys(e.Jobs).length
        }
    })
    result.sort((a,b) => a.jobsCount > b.jobsCount ? 1 : -1)
    console.log(JSON.stringify(result))
})
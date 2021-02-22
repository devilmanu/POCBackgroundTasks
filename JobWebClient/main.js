// @ts-ignore
class Job {
    /** @type {string} */
    id
    /** @type {string} */
    status    
    /** @type {string} */
    name 
    /** @type {string} */
    progress 
}
     


class JobStore {
    constructor() {
        this.jobsCompleted = new Map();
        this.jobsInProgress = new Map();
    }
    /**
     * @param {Job} job
     */
    findJobInProgress = (job) => this.jobsInProgress.get(job.id);
    /**
     * @param {Job} job
     */
    findJobCompleted = (job) => this.jobsInProgress.get(job.id);
    /**
     * @param {Job} job
     */
    addJobCompleted = (job) => this.jobsInProgress.set(job.id, job);
    /**
     * @param {Job} job
     */
    addJobInProgress  = (job) => this.jobsInProgress.set(job.id, job);;
        /**
     * @param {Job} job
     */
    updateJobCompleted = (job) => this.jobsInProgress.set(job.id, job);
    /**
     * @param {Job} job
     */
    updateJobInProgress  = (job) => this.jobsInProgress.set(job.id, job);;
    /**
     * @param {Job} job
     */
    removeJobInProgress = (job) => this.jobsInProgress.delete(job.id);
    /**
     * @param {Job} job
     */
    removeJobCompleted = (job) => this.jobsInProgress.delete(job.id);
}

const store = new JobStore();


document.addEventListener("DOMContentLoaded",() => {

});


// @ts-ignore
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5003/jobhub")
    // @ts-ignore
    .configureLogging(signalR.LogLevel.Trace)
    .build();

/**
 * @param {any} user
 * @param {any} message
 */
connection.on("sendprogress", (message) => {
    //feoooo mapeo
    let newJob = new Job()
    newJob.name = message.jobName
    newJob.id = message.jobId
    newJob.progress = message.progress
    newJob.status = message.status

    let jobFound = store.findJobInProgress(newJob)
    if(jobFound){
        store.updateJobInProgress(jobFound)
        updateProgressBar(newJob)
    } 
    else {  
        store.addJobInProgress(newJob)
        createProgressBar(newJob)
    }
});

/**
 * @param {{ toString: () => any; }} err
 */
connection.start().catch(err => console.error(err.toString()));



const updateProgressBar = (job) => {
    let ele = document.getElementById(job.id);
    let bar = ele.childNodes[0].childNodes[0];
    // @ts-ignore
    bar.setAttribute('aria-valuenow', job.progress.replace('%', ''));
    // @ts-ignore
    bar.setAttribute('style',`width: ${job.progress}`);
    // @ts-ignore
    bar.innerHTML = `${job.progress} ${job.status}`
}
const createProgressBar = (job) => {
    let container = document.getElementById("jobsinprogresscontainer")

    let div = document.createElement('div');
    div.className = 'mt-3';
    div.id = job.id;
    

    let h6 = document.createElement('h6');
    h6.innerHTML = `${job.id} ${job.name}`

    
    let divprogress = document.createElement('div');
    divprogress.className = 'progress';

    let divprogressbar = document.createElement('div');
    divprogressbar.className = 'progress-bar progress-bar-striped progress-bar-animated';
    divprogressbar.setAttribute('role','progressbar');
    divprogressbar.setAttribute('aria-valuenow','0');
    divprogressbar.setAttribute('aria-valuemin','0');
    divprogressbar.setAttribute('aria-valuemax','100');
    divprogressbar.setAttribute('style',"width: 0%");

    divprogress.appendChild(divprogressbar)
    div.appendChild(divprogress)
    div.appendChild(h6)
    container.appendChild(div);
}
const deleteProgressBar = (job) => {
    document.getElementById(job.id).remove();
}


const ID = () => {
    // Math.random should be unique because of its seeding algorithm.
    // Convert it to base 36 (numbers + letters), and grab the first 9 characters
    // after the decimal.
    return '_' + Math.random().toString(36).substr(2, 9);
  };

const createJob = () => {
    // @ts-ignore
    axios.post('https://localhost:5001/api/jobs', {
        jobName: document.getElementById('inputcreate').nodeValue,
        jobId : ID
    }).then()
}
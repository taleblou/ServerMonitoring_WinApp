# **ServerMonitor**

`ServerMonitor` is a Windows Forms application designed to monitor the performance of multiple URLs by checking their ping time, loading time, response size, status code, and error message. It saves the collected data into a CSV file for record-keeping. The application also provides real-time monitoring, visualizing the status of each URL with progress bars, and alerts with a sound notification when the status code is not 200\.

## **Features**

* Monitor up to 6 URLs simultaneously.  
* Track key metrics: Ping Time (ms), Loading Time (s), Response Size (bytes), Status Code, and Error Message.  
* Save monitoring data to a CSV file.  
* Visualize URL statuses with color-coded progress bars.  
* Alerts when a URL is down (non-200 status code).  
* Customizable monitoring interval.  
* Log monitoring results and errors in a text box.

## **Requirements**

* .NET Framework (Windows Forms application).  
* A valid sound file (e.g., WAV) for alerts (optional).

## **Setup**

1. Clone or download the repository.  
2. Open the project in Visual Studio.  
3. Ensure you have a valid sound file for alerts (or remove the `PlaySound()` function if not needed).  
4. Build and run the project.

## **Usage**

* Enter the URLs you wish to monitor in the provided text boxes.  
* Set the monitoring time interval (in seconds) using the `Time_TextBox`.  
* Click the **Start Monitoring** button to begin monitoring.  
  * The button text will change to **Stop Monitoring** while monitoring is active.  
* The application will ping each URL and check its status every defined interval.  
* Data is saved in a CSV file (`url_reports.csv`), with the following columns:  
  * Date  
  * Ping Time (ms)  
  * Loading Time (s)  
  * Response Size (bytes)  
  * Status Code  
  * Error Message  
* The monitoring results are displayed in a log box for real-time feedback.

## **Code Explanation**

### **Main Class**

The main class `Main` contains the logic for initializing the application, starting and stopping monitoring, and handling URL checks and logging.

* **Variables**:  
  * `isMonitoring`: Flag to track if monitoring is active.  
  * `time`: Monitoring interval in seconds.  
  * `csvFile`: The file where monitoring results are saved.  
  * `urlTextBoxes`: Array of text boxes for URLs.  
  * `progressBar`: Array of progress bars for visualizing the status of the monitored URLs.  
* **Start Monitoring**: When the **Start Monitoring** button is clicked, the application begins monitoring the specified URLs at the interval set in `Time_TextBox`. It checks the URL's status and logs the results to a CSV file.  
* **PingHostAsync**: Asynchronously pings each URL to measure response time.  
* **CheckUrlAsync**: Asynchronously checks the status of each URL, including loading time, response size, and status code.  
* **NormalizeLoadingTime**: Normalizes the loading time to a percentage for the progress bar.  
* **PlaySound**: Plays an alert sound when a URL returns a status code other than 200 (optional).

## **CSV Format**

The CSV file will contain the following columns:

* `Date`: The timestamp of the monitoring check.  
* `Ping Time (ms)`: The time taken for the ping request.  
* `Loading Time (s)`: The time taken to load the URL.  
* `Response Size (bytes)`: The size of the response in bytes.  
* `Status Code`: The HTTP status code returned.  
* `Error Message`: Any error encountered during the check.

## **Notes**

* The application supports up to 6 URLs.  
* The `url_reports.csv` file is created in the same directory as the application if it doesn't already exist.  
* The progress bars change color based on the status: green for a successful response (status code 200\) and red for an unsuccessful response.


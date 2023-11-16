using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    // private GameManager manager;
    [SerializeField] private Transform myTransform;
    private List<GameObject> drops;
    private List<GameObject> finalClusters;
    private GameObject destination;
    [SerializeField] float centroidChangeRange;
    [SerializeField] int clusterNumbers;
   // [SerializeField] int carryCapacity;
    [SerializeField] GameObject centroidPrefab;

    // Start is called before the first frame update
    void Start() {
        // initialize manager
        // manager = GameObject.FindObjectWithTag("GameManager").GetComponent<GameManager>();

        // get a list of all the drops
        drops = new List<GameObject>(GameObject.FindGameObjectsWithTag("Drop"));
        foreach (GameObject drop in drops) {
            Debug.Log("*******Drop**********");
        }

        // run clustering algorithm to find where the clusters are on the screen
        finalClusters = ClusterBehavior(drops, clusterNumbers);

        // get destination to move to
        /*if(finalClusters.Count == 0) {
            destination = GameObject.FindGameObjectWithTag("Player").transform.position;
        } else {
            destination = GetDestination(finalClusters);
        }*/

    }

    /// <summary>
    /// Clustering algorithm that finds the location of k number of groups on the screen.
    /// </summary>
    /// <param name="objects">Objects that you want to group (i.e. drop objects)</param>
    /// <param name="numberOfClusters">Number of groupings</param>
    /// <returns>A list of vectors indicating where the groups are located.</returns>
    private List<GameObject> ClusterBehavior(List<GameObject> objects, int numberOfClusters) {

        // CREATE N CENTROIDS AND RANDOMLY SET THEIR VALUES

        // declare max of random positions on world screen
        int x_max = 9;
        int y_max = 4;

        List<GameObject> centroids = new List<GameObject>();
        // Check if there are no objects to go to
        if(objects.Count == 0) {
            return centroids;
        }
        for(int i = 0; i < numberOfClusters; i++) {
            Vector3 randomLocation = new Vector3(Random.value * x_max, Random.value * y_max, 0.0f);
            GameObject newCentroid = Instantiate(centroidPrefab, randomLocation, Quaternion.identity);
            centroids.Add(newCentroid);

            Debug.Log("New Centroid Created With Coords of X: " + newCentroid.transform.position.x + " Y: " + newCentroid.transform.position.y );
        }
        List<GameObject> oldCentroids = new List<GameObject>();


        // CREATE A LIST THAT HOLDS EACH CLUSTER (EACH INDEX CORRELATES TO CENTROIDS)
        List<List<GameObject>> clusters = new List<List<GameObject>>();
        for(int i = 0; i < numberOfClusters; i++) {
            clusters.Add(new List<GameObject>());
        }
        
        // ITTERATE UNTIL THE CENTROIDS ARE IN THE CENTER OF THE CLUSTERS
        bool clusterDone = false;

        while(!clusterDone) {    
            // GET THE DISTANCE BETWEEN EVERY POINT [GAME OBJECT] TO EVERY CENTROID
            // AND STORE POINT TO THE CLOSEST CLUSTER. (CLUSTER N CORRELATES TO CENTROID N).
            Debug.Log("ITERATION***********");

            // clear clusters for itteration
            clusters = ClearClusters(clusters);

            // keep track of closest cluster/centroid
            float bestDistance;        
            int indexOfBestDistance;
            
            foreach (GameObject point in objects) {
                bestDistance = Mathf.Infinity;
                indexOfBestDistance = -1;

                for (int i = 0; i < numberOfClusters; i++){
                    // distance between point and centroid i
                    float curDistance = EuclideanDistance(point.transform.position, centroids[i].transform.position);

                    // if current distance is smaller than best distance than change best distance
                    if(curDistance < bestDistance) {
                        bestDistance = curDistance;
                        indexOfBestDistance = i;
                    }
                }

                // Add point into closest cluster
                if(indexOfBestDistance == -1) {
                    Debug.Log("No best cluster was found for a drop");
                } else {
                    clusters[indexOfBestDistance].Add(point);
                }
            }


            // UPDATE CENTROID POSITION BASE ON AVERAGE POSITION OF POINTS IN CLUSTER
            oldCentroids = centroids;

            centroids = UpdateCentroids(centroids, clusters);

            // determine if we should break
            clusterDone = CentroidsInRange(centroids, oldCentroids, centroidChangeRange);
        }

        for (int i = 0; i < clusters.Count; i++) {

            foreach (GameObject drop in clusters[i]) {
                if(i == 0) {
                    drop.GetComponent<Renderer>().material.color = Color.cyan;
                }
                if(i == 1) {
                    drop.GetComponent<Renderer>().material.color = Color.magenta;
                }
                if(i == 2) {
                    drop.GetComponent<Renderer>().material.color = Color.yellow;
                }
            }
        }

        return centroids;

    }

    
    /// <summary>
    /// Finds the distance between two Vector3 points
    /// </summary>
    /// <param name="point">Location 1 to compare</param>
    /// <param name="centroid">Location 2 to compare</param>
    /// <returns>Distance between the point and centroid</returns>
        private float EuclideanDistance(Vector3 point, Vector3 centroid) {
        float x_distance = Mathf.Pow((centroid.x - point.x), 2.0f);
        float y_distance = Mathf.Pow((centroid.y - point.y), 2.0f);

        return Mathf.Sqrt(x_distance + y_distance);
    }

    /// <summary>
    /// Set each centroid to the avaregae of the location of all objects within it's current grouping
    /// </summary>
    /// <param name="centroids">Location to be updated</param>
    /// <param name="clusters">Current grouping of objects</param>
    /// <returns>Updated centroids list</returns>
    private List<GameObject> UpdateCentroids(List<GameObject> centroids, List<List<GameObject>> clusters) {
        
         // find the average points for each centroid
        float x_total;
        float y_total;

        // calculate the average of each cluster
        for (int i = 0; i < clusters.Count; i++) {
            x_total = 0.0f;
            y_total = 0.0f;

            // Compute the x and y totals positons for each point in a cluster
             foreach (GameObject point in clusters[i]) {
                x_total += point.transform.position.x;
                y_total += point.transform.position.y;
             }

            // set centroid to average of x and y total of a cluster
            Vector3 newCentroidLocation = new Vector3(x_total / clusters[i].Count, y_total / clusters[i].Count, 0);
            centroids[i].transform.position = newCentroidLocation;
        }

        foreach (GameObject centroid in centroids) {
            Debug.Log("Centroid update to X: " + centroid.transform.position.x + " Centroid update to Y: " + centroid.transform.position.y); 
        }

        return centroids;
    } 

    /// <summary>
    /// Empty cluster list of all objects for regrouping.
    /// </summary>
    /// <param name="clusters">Current grouping set up</param>
    /// <returns>Empty cluster lists</returns>
    private List<List<GameObject>> ClearClusters(List<List<GameObject>> clusters) {
        foreach (List<GameObject> cluster in clusters) {
            cluster.Clear();
        }
        return clusters;
    }

    /// <summary>
    /// Check if centroid location has drastically changed. If it has not, then stop running the
    /// clustering algorithm.
    /// </summary>
    /// <param name="newCentroids">List of new centroids</param>
    /// <param name="oldCentroids">List of old centroids</param>
    /// <param name="range">Determine the minimal distance needed to stop algorithm</param>
    /// <returns>True if the algorithm can stop and false if the algorithm must run again.</returns>
    private bool CentroidsInRange(List<GameObject> newCentroids, List<GameObject> oldCentroids, float range) {
        float[] centroidDifference = new float[clusterNumbers];

        // find the distance for every variable and retrun if out of range
        for(int i = 0; i < clusterNumbers; i++) {

            Debug.Log("new centroid x: " + newCentroids[i].transform.position.x);
            Debug.Log("old centroid x: " + oldCentroids[i].transform.position.x);
            //centroidDifference[i] = EuclideanDistance(newCentroids[i].transform.position, oldCentroids[i].transform.position);
            float test = newCentroids[i].transform.position.x - oldCentroids[i].transform.position.x;
            
            Debug.Log("Difference: " + test);
            
            if(centroidDifference[i] > range) {
                Debug.Log("Must go through another itteration");
                return false;
            }

            Debug.Log("In range " + i);
        }

        // if all centroids are in range then return true
        return true;

    }

    /// <summary>
    /// Using the clusters, determine which cluster is the closest.
    /// </summary>
    /// <param name="centroids">Location of the cluster</param>
    /// <returns>The cluster closest to the player</returns>
    private GameObject GetDestination(List<GameObject> centroids) {
        // go to location with most amount of drops and closest

        float bestDistance = Mathf.Infinity;
        float distanceToCluster = Mathf.Infinity;

        int bestLocationIndex = -1;

        for (int i = 0; i < centroids.Count; i++) {

            // determine the weight of a current cluster
            //distanceToCluster =  Mathf.Pow((EuclideanDistance(self, centroid) / clusters[i].Count), 2.0f);
            distanceToCluster = EuclideanDistance(myTransform.position, centroids[i].transform.position);
            
            // if better location found then set as new destination
            if(distanceToCluster < bestDistance) {
                bestDistance = distanceToCluster;
                bestLocationIndex = i;
            } 

        }

        GameObject bestCentroidLocation = centroids[bestLocationIndex];

        return bestCentroidLocation;

    } 

    /// <summary>
    /// Access the destination variable.
    /// </summary>
    /// <returns>Destination of closest cluster</returns>
    public GameObject GetDestination() {
        return destination;
    }


}

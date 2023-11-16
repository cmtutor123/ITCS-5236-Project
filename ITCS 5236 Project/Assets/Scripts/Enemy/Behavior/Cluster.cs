using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    // private GameManager manager;
    [SerializeField] private Transform myTransform;
    private List<GameObject> drops;
    private List<Vector3> finalClusters;
    private Vector3 destination;
    [SerializeField] float centroidChangeRange;
    [SerializeField] int clusterNumbers;
    [SerializeField] int carryCapacity;

    // Start is called before the first frame update
    void Start() {
        // initialize manager
        // manager = GameObject.FindObjectWithTag("GameManager").GetComponent<GameManager>();

        // get a list of all the drops
        drops = new List<GameObject>(GameObject.FindGameObjectsWithTag("Drop"));
        Debug.Log(drops);

        // run clustering algorithm to find where the clusters are on the screen
       // finalClusters = ClusterBehavior(drops, clusterNumbers);

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
    private List<Vector3> ClusterBehavior(List<GameObject> objects, int numberOfClusters) {

        // CREATE N CENTROIDS AND RANDOMLY SET THEIR VALUES

        // declare max of random positions on world screen
        int x_max = 1000;
        int y_max = 1000;

        List<Vector3> centroids = new List<Vector3>();
        // Check if there are no objects to go to
        if(objects.Count == 0) {
            return centroids;
        }
        List<Vector3> oldCentroids = new List<Vector3>();
        for(int i = 0; i < numberOfClusters; i++) {
            centroids.Add(new Vector3(Random.value * x_max, Random.value * y_max, 0.0f));
        }

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

            foreach (Vector3 centroid in centroids) {
                Debug.DrawLine(centroid, new Vector3(centroid.x + 5, centroid.y, 0) , Color.white, 2.5f);
            }

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
                    float curDistance = EuclideanDistance(point.transform.position, centroids[i]);

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

        return centroids;

    }

    
    /// <summary>
    /// Finds the distance between two Vector3 points
    /// </summary>
    /// <param name="point">Location 1 to compare</param>
    /// <param name="centroid">Location 2 to compare</param>
    /// <returns>Distance between the point and centroid</returns>
        private float EuclideanDistance(Vector3 point, Vector3 centroid) {
        float x_distance = Mathf.Pow((point.x - centroid.x), 2.0f);
        float y_distance = Mathf.Pow((point.y - centroid.y), 2.0f);

        return Mathf.Sqrt(x_distance + y_distance);
    }

    /// <summary>
    /// Set each centroid to the avaregae of the location of all objects within it's current grouping
    /// </summary>
    /// <param name="centroids">Location to be updated</param>
    /// <param name="clusters">Current grouping of objects</param>
    /// <returns>Updated centroids list</returns>
    private List<Vector3> UpdateCentroids(List<Vector3> centroids, List<List<GameObject>> clusters) {
        
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
            Vector3 tempCentroid = new Vector3();

            tempCentroid.x = x_total / clusters[i].Count;
            tempCentroid.y = y_total / clusters[i].Count;

            centroids[i] = tempCentroid;
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

    // check if the centroid location is drastically changing. If not, then stop running the
    // clustering algorithm
    /// <summary>
    /// Check if centroid location has drastically changed. If it has not, then stop running the
    /// clustering algorithm.
    /// </summary>
    /// <param name="newCentroids">List of new centroids</param>
    /// <param name="oldCentroids">List of old centroids</param>
    /// <param name="range">Determine the minimal distance needed to stop algorithm</param>
    /// <returns>True if the algorithm can stop and false if the algorithm must run again.</returns>
    private bool CentroidsInRange(List<Vector3> newCentroids, List<Vector3> oldCentroids, float range) {
        float[] centroidDifference = new float[newCentroids.Count];

        // find the distance for every variable and retrun if out of range
        for(int i = 0; i < newCentroids.Count; i++) {
            centroidDifference[i] = EuclideanDistance(newCentroids[i], oldCentroids[i]);
            if(centroidDifference[i] > range) {
                return false;
            }
        }

        // if all centroids are in range then return true
        return true;

    }


    /// <summary>
    /// Using the clusters, determine which cluster is the closest.
    /// </summary>
    /// <param name="centroids">Location of the cluster</param>
    /// <returns>The cluster closest to the player</returns>
    private Vector3 GetDestination(List<Vector3> centroids) {
        // go to location with most amount of drops and closest

        float bestDistance = Mathf.Infinity;
        float distanceToCluster = Mathf.Infinity;

        int bestLocationIndex = -1;

        for (int i = 0; i < centroids.Count; i++) {

            // determine the weight of a current cluster
            //distanceToCluster =  Mathf.Pow((EuclideanDistance(self, centroid) / clusters[i].Count), 2.0f);
            distanceToCluster = EuclideanDistance(myTransform.position, centroids[i]);
            
            // if better location found then set as new destination
            if(distanceToCluster < bestDistance) {
                bestDistance = distanceToCluster;
                bestLocationIndex = i;
            } 

        }

        Vector3 bestCentroidLocation = centroids[bestLocationIndex];

        return bestCentroidLocation;

    } 

    /// <summary>
    /// Access the destination variable.
    /// </summary>
    /// <returns>Destination of closest cluster</returns>
    public Vector3 GetDestination() {
        return destination;
    }


}

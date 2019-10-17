# End-to-End Text Recognition with Custom Vision API and Azure OCR Service
This .Net application will take in a batch of images with text objects. The text objects will first be detected by the Custom Vision API service which will return the location of the bounding boxes. 

The application will then crop out all the text objects concerned based on the bounding boxes and save them in a new folder.

The cropped out objects will then be sent in batch directly to Text Recogniser API to perform OCR.

# Instruction and Deployment

This application is written in .Net, please use Visual Studio 2017 or above to build and run the solution.

1. Create a new project in custom vision service ([https://www.customvision.ai/](https://www.customvision.ai/)). Upload at least 15 images for each tag. Train and publish your custom model as an API Endpoint.

![123](https://user-images.githubusercontent.com/17831550/66981425-41e23980-f0e6-11e9-9e97-152898607bff.PNG)

2. Specify your Endpoint and key of your custom vision service and Recogniser Text service in the app.config file. Specify also the tags in concern.
![123123](https://user-images.githubusercontent.com/17831550/66981503-82da4e00-f0e6-11e9-905a-20ce5aa6b818.PNG)

3. We are using the threading technique to create a queue for processing the images in batch. Depending on the pricing tier of your cognitive services, we can process up to 20 images per second.
![123123](https://user-images.githubusercontent.com/17831550/66981590-c03edb80-f0e6-11e9-8f19-665fc7bf092d.PNG)

4. We are looking for the specified tag with the highest probability here and then we extract the coordinates for cropping.
![123131](https://user-images.githubusercontent.com/17831550/66981654-f41a0100-f0e6-11e9-9520-4c0403cfbed6.PNG)

5. There are 4 status of the Recogniser Text service's Operation, namely "Started", "Not Started", "Succeeded" and "Failed". We are creating a loop here to scan for the success status and output the recognition results.
![12312312](https://user-images.githubusercontent.com/17831550/66981714-19a70a80-f0e7-11e9-9b59-1d73599ff8d0.PNG)

6. Specify the source folder which contains all of your testing images

7. Specify the destination folder for storing your cropped images.

8. Sample console output of a successful execution.

![123123123](https://user-images.githubusercontent.com/17831550/66982179-35f77700-f0e8-11e9-9dc3-3d31bd06f01f.PNG)




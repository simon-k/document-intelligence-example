<template>
  <div class="app-container">
    <header class="header">
      <h1>🏋️ Fitness Exercise Analyzer</h1>
      <p>Upload your fitness exercise images to extract exercise data using AI Document Intelligence</p>
    </header>

    <main class="main-content">
      <div class="upload-section">
        <div class="upload-card">
          <h2>Upload Exercise Image</h2>
          
          <form @submit.prevent="uploadFile" class="upload-form">
            <div class="file-input-container">
              <label for="file-input" class="file-input-label">
                <div class="file-input-content">
                  <div class="upload-icon">📁</div>
                  <div v-if="!selectedFile" class="upload-text">
                    Click to select an image file
                  </div>
                  <div v-else class="selected-file">
                    <strong>Selected:</strong> {{ selectedFile.name }}
                    <br>
                    <small>{{ formatFileSize(selectedFile.size) }}</small>
                  </div>
                </div>
              </label>
              <input 
                id="file-input"
                type="file" 
                @change="onFileSelect"
                accept="image/*"
                class="file-input"
                :disabled="isUploading"
              />
            </div>

            <button 
              type="submit" 
              :disabled="!selectedFile || isUploading"
              class="upload-button"
            >
              <span v-if="isUploading" class="button-content">
                <span class="spinner"></span>
                Analyzing...
              </span>
              <span v-else>🚀 Analyze Exercise</span>
            </button>
          </form>

          <div v-if="uploadProgress" class="progress-container">
            <div class="progress-bar">
              <div class="progress-fill" :style="{ width: uploadProgress + '%' }"></div>
            </div>
            <small>{{ uploadProgress }}% complete</small>
          </div>
        </div>
      </div>

      <!-- Results Section -->
      <div v-if="analysisResult" class="results-section">
        <div class="results-card">
          <h2>📊 Analysis Results</h2>
          <div v-if="analysisResult.type" class="workout-summary">
            <div class="workout-header">
              <h3>{{ analysisResult.type }}</h3>
              <div v-if="analysisResult.duration" class="duration">
                <strong>Duration:</strong> {{ analysisResult.duration }}
              </div>
            </div>
            <div v-if="analysisResult.phases && analysisResult.phases.length > 0" class="phases-list">
              <div v-for="(phase, index) in analysisResult.phases" :key="index" class="phase-item">
                <h4>{{ phase.name }}</h4>
                <ul v-if="phase.tasks && phase.tasks.length > 0" class="tasks-list">
                  <li v-for="(task, taskIndex) in phase.tasks" :key="taskIndex" class="task-item">
                    {{ task }}
                  </li>
                </ul>
              </div>
            </div>
          </div>
          <div v-else class="no-exercises">
            <p>No workout data detected in the uploaded image.</p>
          </div>
        </div>
      </div>

      <!-- Error Section -->
      <div v-if="error" class="error-section">
        <div class="error-card">
          <h2>❌ Error</h2>
          <p>{{ error }}</p>
          <button @click="clearError" class="clear-error-button">Clear</button>
        </div>
      </div>
    </main>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'App',
  data() {
    return {
      selectedFile: null,
      isUploading: false,
      uploadProgress: null,
      analysisResult: null,
      error: null
    }
  },
  methods: {
    onFileSelect(event) {
      const file = event.target.files[0]
      if (file) {
        // Validate file type
        if (!file.type.startsWith('image/')) {
          this.error = 'Please select an image file (JPG, PNG, etc.)'
          return
        }
        
        // Validate file size (10MB limit)
        if (file.size > 10 * 1024 * 1024) {
          this.error = 'File size must be less than 10MB'
          return
        }

        this.selectedFile = file
        this.error = null
        this.analysisResult = null
      }
    },

    async uploadFile() {
      if (!this.selectedFile) return

      this.isUploading = true
      this.uploadProgress = 0
      this.error = null
      this.analysisResult = null

      try {
        const formData = new FormData()
        formData.append('file', this.selectedFile)

        const response = await axios.post('/api/upload', formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          },
          onUploadProgress: (progressEvent) => {
            if (progressEvent.lengthComputable) {
              this.uploadProgress = Math.round((progressEvent.loaded * 100) / progressEvent.total)
            }
          }
        })

        this.analysisResult = response.data
        this.uploadProgress = null
        
      } catch (error) {
        console.error('Upload error:', error)
        this.error = error.response?.data?.message || 
                   error.response?.data || 
                   'Failed to analyze the image. Please try again.'
        this.uploadProgress = null
      } finally {
        this.isUploading = false
      }
    },

    formatFileSize(bytes) {
      if (bytes === 0) return '0 Bytes'
      const k = 1024
      const sizes = ['Bytes', 'KB', 'MB', 'GB']
      const i = Math.floor(Math.log(bytes) / Math.log(k))
      return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
    },

    clearError() {
      this.error = null
    }
  }
}
</script>

<style scoped>
.app-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  text-align: center;
  margin-bottom: 40px;
}

.header h1 {
  color: #2c3e50;
  margin-bottom: 10px;
  font-size: 2.5rem;
}

.header p {
  color: #7f8c8d;
  font-size: 1.1rem;
  margin: 0;
}

.upload-card, .results-card, .error-card {
  background: white;
  border-radius: 12px;
  padding: 30px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;
}

.upload-card h2, .results-card h2, .error-card h2 {
  margin-top: 0;
  color: #2c3e50;
  font-size: 1.5rem;
}

.file-input-container {
  margin-bottom: 20px;
}

.file-input-label {
  display: block;
  cursor: pointer;
  border: 2px dashed #3498db;
  border-radius: 8px;
  padding: 40px 20px;
  text-align: center;
  transition: all 0.3s ease;
  background-color: #f8f9fa;
}

.file-input-label:hover {
  border-color: #2980b9;
  background-color: #e3f2fd;
}

.file-input {
  display: none;
}

.upload-icon {
  font-size: 3rem;
  margin-bottom: 10px;
}

.upload-text {
  color: #3498db;
  font-size: 1.1rem;
  font-weight: 500;
}

.selected-file {
  color: #27ae60;
  font-size: 1rem;
}

.upload-button {
  width: 100%;
  padding: 15px;
  background-color: #3498db;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.upload-button.uploading,
.upload-button.uploading:disabled {
  background-color: #3498db !important;
  cursor: progress;
}

.upload-button:hover:not(:disabled) {
  background-color: #2980b9;
}

.upload-button:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}

.progress-container {
  margin-top: 15px;
  text-align: center;
}

.progress-bar {
  width: 100%;
  height: 8px;
  background-color: #ecf0f1;
  border-radius: 4px;
  overflow: hidden;
  margin-bottom: 5px;
}

.progress-fill {
  height: 100%;
  background-color: #3498db;
  transition: width 0.3s ease;
}

.exercises-list {
  display: grid;
  gap: 15px;
}

.exercise-item {
  border: 1px solid #e1e8ed;
  border-radius: 8px;
  padding: 20px;
  background-color: #f8f9fa;
}

.exercise-item h3 {
  margin: 0 0 15px 0;
  color: #2c3e50;
  font-size: 1.2rem;
}

.exercise-details {
  display: grid;
  gap: 8px;
}

.detail-item {
  color: #5a6c7d;
}

.detail-item strong {
  color: #2c3e50;
}

.no-exercises {
  text-align: center;
  color: #7f8c8d;
  font-style: italic;
}

.error-card {
  background-color: #ffeaea;
  border-left: 4px solid #e74c3c;
}

.error-card h2 {
  color: #c0392b;
}

.error-card p {
  color: #a93226;
  margin-bottom: 15px;
}

.clear-error-button {
  padding: 8px 16px;
  background-color: #e74c3c;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
}

.clear-error-button:hover {
  background-color: #c0392b;
}

.results-section, .error-section {
  margin-top: 20px;
}

.workout-summary {
  margin-bottom: 20px;
}

.workout-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 15px;
  border-bottom: 2px solid #ecf0f1;
}

.workout-header h3 {
  margin: 0;
  color: #2c3e50;
  font-size: 1.4rem;
  font-weight: 600;
}

.duration {
  font-size: 1rem;
  color: #7f8c8d;
  background-color: #f8f9fa;
  padding: 5px 12px;
  border-radius: 20px;
  border: 1px solid #e1e8ed;
}

.phases-list {
  display: grid;
  gap: 20px;
}

.phase-item {
  border: 1px solid #e1e8ed;
  border-radius: 8px;
  padding: 20px;
  background-color: #f8f9fa;
}

.phase-item h4 {
  margin: 0 0 15px 0;
  color: white;
  font-size: 1.1rem;
  font-weight: 600;
  padding: 8px 12px;
  background-color: #3498db;
  border-radius: 6px;
  display: inline-block;
}

.tasks-list {
  list-style-type: none;
  padding: 0;
  margin: 0;
  display: grid;
  gap: 8px;
}

.task-item {
  font-size: 0.95rem;
  color: #34495e;
  padding: 8px 12px;
  background-color: white;
  border-radius: 6px;
  border-left: 3px solid #3498db;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.spinner {
  width: 20px;
  height: 20px;
  border: 3px solid rgba(255, 255, 255, 0.8);
  border-top: 3px solid #3498db;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
  margin-right: 10px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>

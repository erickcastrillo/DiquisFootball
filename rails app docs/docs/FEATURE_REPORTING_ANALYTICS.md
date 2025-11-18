# Diquis - Reporting & Analytics Feature

## Overview

The Reporting & Analytics module provides comprehensive business intelligence capabilities for football academies.
This system generates actionable insights across all aspects of academy operations, from financial performance to
player development analytics. Reports are available at both academy-specific and system-wide levels for
multi-academy organizations.

## Core Functionality

### Report Categories

#### 1. Financial Reports

- **Revenue Analysis**: Income tracking by source (registration fees, merchandise, camps)
- **Expense Tracking**: Operating costs, equipment, staff salaries, facility maintenance
- **Profit & Loss Statements**: Monthly, quarterly, and annual P&L reports
- **Budget vs. Actual**: Variance analysis and budget performance
- **Cash Flow Reports**: Incoming and outgoing cash flow projections
- **Player Fee Management**: Outstanding payments, payment history, late fees

#### 2. Player Analytics

- **Development Tracking**: Individual player progress over time
- **Performance Metrics**: Skill assessments, training attendance, improvement rates
- **Player Retention**: Registration trends, dropout analysis, retention strategies
- **Age Group Analysis**: Performance and participation by age category
- **Position Analysis**: Player distribution and development by position
- **Health & Injury Reports**: Injury tracking, recovery times, prevention metrics

#### 3. Team Performance Reports

- **Team Statistics**: Performance metrics by team and category
- **Attendance Reports**: Training and match attendance patterns
- **Coaching Effectiveness**: Player improvement under different coaches
- **Team Composition**: Player distribution, balance, and roster optimization
- **Competition Results**: Match results, tournament performance, league standings

#### 4. Operational Reports

- **Facility Utilization**: Field usage, capacity optimization, scheduling efficiency
- **Equipment & Asset Reports**: Asset utilization, maintenance costs, depreciation
- **Staff Performance**: Coach ratings, staff productivity, training effectiveness
- **Training Analysis**: Session types, frequency, effectiveness metrics
- **Communication Metrics**: Parent engagement, notification effectiveness

#### 5. Business Intelligence

- **Academy Benchmarking**: Performance comparison across academies
- **Market Analysis**: Local competition, pricing analysis, market positioning
- **Growth Projections**: Enrollment forecasts, revenue predictions
- **KPI Dashboards**: Key performance indicators and trends
- **Custom Analytics**: Flexible reporting with user-defined metrics

## Data Models

### Report

```ruby
class Report < ApplicationRecord
  include UuidPrimaryKey
  acts_as_tenant(:academy)
  
  belongs_to :academy
  belongs_to :created_by, class_name: 'User'
  has_many :report_generations, dependent: :destroy
  has_many :report_schedules, dependent: :destroy
  has_many :report_subscribers, dependent: :destroy
  
  validates :name, presence: true
  validates :report_type, presence: true
  validates :query_template, presence: true
  
  enum report_type: {
    financial_summary: 0,
    player_development: 1,
    attendance_analysis: 2,
    coaching_effectiveness: 3,
    churn_analysis: 4,
    growth_tracking: 5,
    retention_analysis: 6,
    lifetime_value: 7,
    acquisition_analysis: 8,
    seasonal_trends: 9,
    competitive_analysis: 10,
    program_effectiveness: 11,
    operational_efficiency: 12,
    parent_satisfaction: 13,
    custom: 99
  }
  
  enum status: {
    active: 0,
    inactive: 1,
    archived: 2
  }
end
```text

### PlayerChurnEvent

```ruby
class PlayerChurnEvent < ApplicationRecord
  include UuidPrimaryKey
  acts_as_tenant(:academy)
  
  belongs_to :academy
  belongs_to :player
  belongs_to :recorded_by, class_name: 'User', optional: true
  
  validates :churn_date, presence: true
  validates :churn_reason, presence: true
  validates :churn_category, presence: true
  
  enum churn_category: {
    cost_related: 0,
    schedule_conflicts: 1,
    coaching_issues: 2,
    player_satisfaction: 3,
    family_circumstances: 4,
    competitive_level: 5,
    facility_issues: 6,
    administrative: 7,
    other: 99
  }
  
  enum churn_type: {
    voluntary: 0,
    involuntary: 1,
    graduated: 2
  }
end
```text

### PlayerRetentionMetric

```ruby
class PlayerRetentionMetric < ApplicationRecord
  include UuidPrimaryKey
  acts_as_tenant(:academy)
  
  belongs_to :academy
  belongs_to :player
  
  validates :enrollment_date, presence: true
  validates :cohort_id, presence: true
  
  # Calculated fields updated by background job
  # 30_day_retained, 90_day_retained, 180_day_retained, 1_year_retained
  # current_stage, satisfaction_score, risk_score
end
```text

### AcquisitionChannel

```ruby
class AcquisitionChannel < ApplicationRecord
  include UuidPrimaryKey
  acts_as_tenant(:academy)
  
  belongs_to :academy
  has_many :player_acquisitions, dependent: :destroy
  has_many :players, through: :player_acquisitions
  
  validates :name, presence: true
  validates :channel_type, presence: true
  
  enum channel_type: {
    referral: 0,
    social_media: 1,
    website: 2,
    local_advertising: 3,
    school_partnership: 4,
    event_marketing: 5,
    word_of_mouth: 6,
    direct_inquiry: 7,
    other: 99
  }
end
```text

### PlayerAcquisition

```ruby
class PlayerAcquisition < ApplicationRecord
  include UuidPrimaryKey
  acts_as_tenant(:academy)
  
  belongs_to :academy
  belongs_to :player
  belongs_to :acquisition_channel
  belongs_to :attributed_to, class_name: 'User', optional: true
  
  validates :acquisition_date, presence: true
  validates :acquisition_cost, presence: true, numericality: { greater_than_or_equal_to: 0 }
  
  # Funnel tracking
  # inquiry_date, trial_date, enrollment_date
  # conversion_time_days, total_touchpoints
end
```text

### CustomerLifetimeValue

```ruby
class CustomerLifetimeValue < ApplicationRecord
  include UuidPrimaryKey
  acts_as_tenant(:academy)
  
  belongs_to :academy
  belongs_to :player
  
  validates :calculated_date, presence: true
  validates :current_clv, presence: true, numericality: { greater_than_or_equal_to: 0 }
  validates :predicted_clv, presence: true, numericality: { greater_than_or_equal_to: 0 }
  
  # Historical tracking of CLV changes
  # months_active, total_revenue, avg_monthly_revenue
  # churn_probability, value_segment
end
```text

#### ReportGeneration

```ruby
class ReportGeneration < ApplicationRecord
  include UuidPrimaryKey
  acts_as_tenant(:academy)
  
  belongs_to :academy
  belongs_to :report
  belongs_to :requested_by, class_name: 'User'
  
  validates :date_range_start, presence: true
  validates :date_range_end, presence: true
  validates :format, presence: true
  
  enum status: {
    queued: 0,
    processing: 1,
    completed: 2,
    failed: 3,
    cancelled: 4
  }
  
  enum format: {
    pdf: 0,
    excel: 1,
    csv: 2,
    json: 3
  }
  
  scope :recent, -> { order(created_at: :desc) }
  scope :successful, -> { where(status: :completed) }
  scope :pending, -> { where(status: [:queued, :processing]) }
  scope :scheduled, -> { where(is_scheduled: true) }
  scope :downloadable, -> { where(status: :completed).where('expires_at > ? OR expires_at IS NULL', Time.current) }
  
  # Database fields for async processing:
  # - started_at :datetime
  # - completed_at :datetime
  # - delivered_at :datetime
  # - file_url :string
  # - file_size :bigint
  # - error_message :text
  # - parameters :jsonb
  # - is_scheduled :boolean, default: false
  # - download_count :integer, default: 0
  # - expires_at :datetime
  # - job_id :string (Sidekiq job ID for tracking)
  # - priority :integer, default: 0
  # - estimated_completion_time :datetime
  
  def processing_time
    return nil unless started_at && completed_at
    ((completed_at - started_at) / 1.minute).round(2)
  end
  
  def expired?
    expires_at && expires_at < Time.current
  end
  
  def downloadable?
    completed? && !expired? && file_url.present?
  end
  
  def increment_download_count!
    increment!(:download_count)
  end
  
  def estimated_size_mb
    case format
    when 'pdf'
      calculate_pdf_size_estimate
    when 'excel'
      calculate_excel_size_estimate
    when 'csv'
      calculate_csv_size_estimate
    else
      1 # Default estimate
    end
  end
  
  def queue_for_generation!
    self.status = :queued
    self.estimated_completion_time = Time.current + estimated_processing_time
    save!
    
    job_id = ReportGenerationJob.perform_async(id)
    update!(job_id: job_id)
  end
  
  def cancel!
    if queued? || processing?
      # Cancel Sidekiq job if possible
      Sidekiq::Cron::Job.find(job_id)&.delete if job_id
      update!(status: :cancelled, completed_at: Time.current)
    end
  end
  
  private
  
  def estimated_processing_time
    # Estimate based on report type and data size
    case report.report_type
    when 'churn_analysis', 'growth_tracking'
      5.minutes
    when 'financial_summary', 'player_development'
      3.minutes
    when 'operational_efficiency', 'parent_satisfaction'
      7.minutes
    else
      4.minutes
    end
  end
  
  def calculate_pdf_size_estimate
    # Estimate PDF size based on data complexity
    base_size = 0.5 # MB
    data_rows = estimate_data_rows
    base_size + (data_rows * 0.001) # ~1KB per row
  end
  
  def calculate_excel_size_estimate
    # Excel files are typically larger
    calculate_pdf_size_estimate * 1.5
  end
  
  def calculate_csv_size_estimate
    # CSV files are smaller
    calculate_pdf_size_estimate * 0.3
  end
  
  def estimate_data_rows
    # Estimate based on academy size and date range
    days = (date_range_end - date_range_start).to_i
    academy.players.count * days * 0.1 # Rough estimate
  end
end
```text

#### ReportSubscription

```ruby
class ReportSubscription < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, optional)
  - report_id (foreign key, required)
  - user_id (foreign key, required)
  - delivery_method (enum: email, dashboard, both)
  - email_address (string) # override user email if needed
  - is_active (boolean, default: true)
  - last_delivered_at (datetime)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy, optional: true
  belongs_to :report
  belongs_to :user
  
  # Validations
  validates :delivery_method, inclusion: { in: %w[email dashboard both] }
  validates :user_id, uniqueness: { scope: :report_id }
end
```text

#### FinancialTransaction

```ruby
class FinancialTransaction < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, required)
  - slug (UUID, indexed)
  - transaction_type (enum: income, expense)
  - category (string, required) # registration_fee, equipment, salary, etc.
  - subcategory (string) # specific classification
  - amount (decimal, precision: 15, scale: 2, required)
  - currency (string, default: 'USD')
  - description (text, required)
  - transaction_date (date, required)
  - payment_method (enum: cash, credit_card, bank_transfer, check, online)
  - reference_number (string) # invoice, receipt, or reference number
  - player_id (foreign key, optional) # if related to specific player
  - team_id (foreign key, optional) # if related to specific team
  - asset_id (foreign key, optional) # if related to asset purchase/maintenance
  - vendor_supplier (string) # for expenses
  - notes (text)
  - is_recurring (boolean, default: false)
  - parent_transaction_id (foreign key, optional) # for recurring transactions
  - created_by_user_id (foreign key, required)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :player, optional: true
  belongs_to :team, optional: true
  belongs_to :asset, optional: true
  belongs_to :parent_transaction, class_name: 'FinancialTransaction', optional: true
  belongs_to :created_by, class_name: 'User'
  has_many :child_transactions, class_name: 'FinancialTransaction', foreign_key: 'parent_transaction_id'
  
  # Scopes
  scope :income, -> { where(transaction_type: 'income') }
  scope :expenses, -> { where(transaction_type: 'expense') }
  scope :by_date_range, ->(start_date, end_date) { where(transaction_date: start_date..end_date) }
  scope :by_category, ->(category) { where(category: category) }
  
  # Validations
  validates :transaction_type, inclusion: { in: %w[income expense] }
  validates :amount, numericality: { greater_than: 0 }
  validates :payment_method, inclusion: { in: %w[cash credit_card bank_transfer check online] }
end
```text

#### PlayerMetric

```ruby
class PlayerMetric < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, required)
  - player_id (foreign key, required)
  - metric_type (string, required) # attendance_rate, skill_improvement, performance_score
  - metric_value (decimal, precision: 10, scale: 4)
  - metric_date (date, required)
  - season (string) # 2025-2026, Fall 2025, etc.
  - notes (text)
  - calculated_at (datetime, required)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :player
  
  # Scopes
  scope :by_metric_type, ->(type) { where(metric_type: type) }
  scope :recent, -> { where('metric_date > ?', 6.months.ago) }
  
  # Validations
  validates :metric_type, presence: true
  validates :metric_value, numericality: true, allow_nil: true
end
```text

#### HomeTrainingSession

```ruby
class HomeTrainingSession < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, required)
  - player_id (foreign key, required)
  - slug (UUID, indexed)
  - generated_at (datetime, required)
  - completed_at (datetime)
  - duration_minutes (integer, required)
  - difficulty_level (enum: beginner, intermediate, advanced, adaptive)
  - focus_areas (jsonb) # ["ball_control", "shooting", "passing"]
  - exercises (jsonb) # Complete exercise data with instructions
  - equipment_required (jsonb) # ["ball", "cones", "wall"]
  - space_requirements (string) # "backyard", "indoor", "park"
  - parent_involvement (boolean, default: false)
  - ai_parameters (jsonb) # Parameters used for AI generation
  - completion_data (jsonb) # Completion feedback and metrics
  - enjoyment_rating (integer) # 1-5 scale
  - difficulty_rating (integer) # 1-5 scale (actual vs expected)
  - improvement_notes (text)
  - next_session_suggestions (jsonb)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :player
  has_many :home_training_exercise_completions, dependent: :destroy
  
  # Scopes
  scope :completed, -> { where.not(completed_at: nil) }
  scope :pending, -> { where(completed_at: nil) }
  scope :recent, -> { where('generated_at > ?', 30.days.ago) }
  scope :by_focus_area, ->(area) { where("focus_areas @> ?", [area].to_json) }
  
  # Validations
  validates :difficulty_level, inclusion: { in: %w[beginner intermediate advanced adaptive] }
  validates :duration_minutes, numericality: { in: 10..60 }
  validates :enjoyment_rating, numericality: { in: 1..5 }, allow_nil: true
  validates :difficulty_rating, numericality: { in: 1..5 }, allow_nil: true
  
  # Methods
  def completion_rate
    return 0 if exercises.blank?
    completed_exercises = completion_data&.dig('exercises_completed') || []
    (completed_exercises.length.to_f / exercises.length * 100).round(1)
  end
  
  def was_enjoyed?
    enjoyment_rating && enjoyment_rating >= 4
  end
  
  def was_appropriate_difficulty?
    difficulty_rating && difficulty_rating.between?(2, 4)
  end
end
```text

#### HomeTrainingExerciseCompletion

```ruby
class HomeTrainingExerciseCompletion < ApplicationRecord
  acts_as_tenant(:academy)
  
  # Attributes
  - academy_id (foreign key, required)
  - home_training_session_id (foreign key, required)
  - exercise_id (integer, required) # ID from the exercises JSON
  - completed_at (datetime, required)
  - sets_completed (integer)
  - sets_planned (integer)
  - duration_seconds (integer)
  - difficulty_rating (integer) # 1-5 scale
  - technique_rating (integer) # 1-5 scale (parent assessment)
  - enjoyment_rating (integer) # 1-5 scale
  - notes (text)
  - video_recorded (boolean, default: false)
  - parent_feedback (text)
  - areas_of_struggle (jsonb) # ["timing", "ball_control", "accuracy"]
  - improvements_noted (jsonb) # ["better_first_touch", "increased_speed"]
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :academy
  belongs_to :home_training_session
  
  # Validations
  validates :exercise_id, presence: true
  validates :difficulty_rating, numericality: { in: 1..5 }, allow_nil: true
  validates :technique_rating, numericality: { in: 1..5 }, allow_nil: true
  validates :enjoyment_rating, numericality: { in: 1..5 }, allow_nil: true
  validates :sets_completed, numericality: { greater_than_or_equal_to: 0 }
  
  # Scopes
  scope :completed_fully, -> { where('sets_completed >= sets_planned') }
  scope :enjoyed, -> { where('enjoyment_rating >= 4') }
  scope :struggled_with, -> { where.not(areas_of_struggle: []) }
end
```text

#### AITrainingTemplate

```ruby
class AITrainingTemplate < ApplicationRecord
  # Note: This is a global model, not tenant-scoped
  
  # Attributes
  - slug (UUID, indexed)
  - name (string, required)
  - description (text)
  - category (string, required) # ball_control, shooting, passing, etc.
  - age_group_min (integer) # minimum age
  - age_group_max (integer) # maximum age
  - difficulty_level (enum: beginner, intermediate, advanced)
  - duration_minutes (integer)
  - equipment_required (jsonb)
  - space_requirements (string)
  - exercise_data (jsonb) # Complete exercise template
  - instruction_video_url (string)
  - coaching_tips (jsonb)
  - progressions (jsonb) # easier/harder variations
  - skill_focus_areas (jsonb) # which skills this exercise targets
  - effectiveness_rating (decimal) # based on user feedback
  - usage_count (integer, default: 0)
  - is_active (boolean, default: true)
  - created_by_user_id (bigint)
  - created_at (datetime)
  - updated_at (datetime)
  
  # Associations
  belongs_to :created_by, class_name: 'User'
  has_many :home_training_session_exercises, dependent: :destroy
  
  # Scopes
  scope :for_age_group, ->(age) { where('age_group_min <= ? AND age_group_max >= ?', age, age) }
  scope :by_category, ->(category) { where(category: category) }
  scope :by_difficulty, ->(level) { where(difficulty_level: level) }
  scope :requiring_equipment, ->(equipment) { where("equipment_required @> ?", [equipment].to_json) }
  scope :highly_rated, -> { where('effectiveness_rating >= 4.0') }
  scope :popular, -> { order(usage_count: :desc) }
  
  # Validations
  validates :name, presence: true
  validates :category, presence: true
  validates :difficulty_level, inclusion: { in: %w[beginner intermediate advanced] }
  validates :age_group_min, :age_group_max, numericality: { in: 5..18 }
  validates :duration_minutes, numericality: { in: 2..20 } # individual exercise duration
  validates :effectiveness_rating, numericality: { in: 1.0..5.0 }, allow_nil: true
  
  # Methods
  def suitable_for_player?(player)
    return false unless is_active?
    return false unless player.age.between?(age_group_min, age_group_max)
    true
  end
  
  def update_effectiveness!(rating)
    current_rating = effectiveness_rating || 3.0
    current_count = usage_count
    new_rating = ((current_rating * current_count) + rating) / (current_count + 1)
    update!(effectiveness_rating: new_rating, usage_count: current_count + 1)
  end
end
```text

## Background Jobs

### ReportGenerationJob

```ruby
class ReportGenerationJob < ApplicationJob
  queue_as :reports
  
  def perform(report_generation_id)
    ActsAsTenant.with_tenant(ReportGeneration.find(report_generation_id).academy) do
      ReportGenerationService.new(report_generation_id: report_generation_id).call
    end
  rescue => e
    Rails.logger.error "Report generation failed: #{e.message}"
    raise e
  end
end
```text

### ReportDeliveryJob

```ruby
class ReportDeliveryJob < ApplicationJob
  queue_as :notifications
  
  def perform(report_generation_id)
    report_generation = ReportGeneration.find(report_generation_id)
    
    ActsAsTenant.with_tenant(report_generation.academy) do
      # Send email to requestor
      ReportMailer.generation_completed(report_generation).deliver_now
      
      # Send to subscribers if it's a scheduled report
      if report_generation.report.report_schedules.any?
        report_generation.report.report_subscribers.active.each do |subscriber|
          ReportMailer.scheduled_report_ready(report_generation, subscriber).deliver_now
        end
      end
      
      # Update delivery status
      report_generation.update!(delivered_at: Time.current)
    end
  rescue => e
    Rails.logger.error "Report delivery failed: #{e.message}"
    raise e
  end
end
```text

### ReportFailureNotificationJob

```ruby
class ReportFailureNotificationJob < ApplicationJob
  queue_as :notifications
  
  def perform(report_generation_id)
    report_generation = ReportGeneration.find(report_generation_id)
    
    ActsAsTenant.with_tenant(report_generation.academy) do
      ReportMailer.generation_failed(report_generation).deliver_now
    end
  rescue => e
    Rails.logger.error "Report failure notification failed: #{e.message}"
    raise e
  end
end
```text

### ScheduledReportJob

```ruby
class ScheduledReportJob < ApplicationJob
  queue_as :scheduled_reports
  
  def perform(report_schedule_id)
    report_schedule = ReportSchedule.find(report_schedule_id)
    
    ActsAsTenant.with_tenant(report_schedule.academy) do
      # Create new report generation
      report_generation = report_schedule.report.report_generations.create!(
        requested_by: report_schedule.created_by,
        status: :queued,
        date_range_start: calculate_start_date(report_schedule),
        date_range_end: calculate_end_date(report_schedule),
        parameters: report_schedule.default_parameters || {},
        format: report_schedule.format || 'pdf',
        is_scheduled: true
      )
      
      # Queue report generation
      ReportGenerationJob.perform_async(report_generation.id)
      
      # Update next run time
      report_schedule.update!(
        last_run_at: Time.current,
        next_run_at: calculate_next_run_time(report_schedule)
      )
    end
  end
  
  private
  
  def calculate_start_date(schedule)
    case schedule.frequency
    when 'daily'
      1.day.ago.beginning_of_day
    when 'weekly'
      1.week.ago.beginning_of_week
    when 'monthly'
      1.month.ago.beginning_of_month
    when 'quarterly'
      3.months.ago.beginning_of_quarter
    when 'yearly'
      1.year.ago.beginning_of_year
    end
  end
  
  def calculate_end_date(schedule)
    case schedule.frequency
    when 'daily'
      1.day.ago.end_of_day
    when 'weekly'
      1.week.ago.end_of_week
    when 'monthly'
      1.month.ago.end_of_month
    when 'quarterly'
      3.months.ago.end_of_quarter
    when 'yearly'
      1.year.ago.end_of_year
    end
  end
  
  def calculate_next_run_time(schedule)
    case schedule.frequency
    when 'daily'
      1.day.from_now
    when 'weekly'
      1.week.from_now
    when 'monthly'
      1.month.from_now
    when 'quarterly'
      3.months.from_now
    when 'yearly'
      1.year.from_now
    end
  end
end
```text

## Mailer Classes

### ReportMailer

```ruby
class ReportMailer < ApplicationMailer
  def generation_completed(report_generation)
    @report_generation = report_generation
    @report = report_generation.report
    @user = report_generation.requested_by
    @academy = report_generation.academy
    
    # Attach report file if small enough for email
    if report_generation.file_size < 10.megabytes
      attachments["#{@report.name}_#{Date.current.strftime('%Y%m%d')}.pdf"] = {
        mime_type: 'application/pdf',
        content: download_file_content(report_generation.file_url)
      }
    end
    
    mail(
      to: @user.email,
      subject: "Report Ready: #{@report.name}",
      template_path: 'report_mailer',
      template_name: 'generation_completed'
    )
  end
  
  def generation_failed(report_generation)
    @report_generation = report_generation
    @report = report_generation.report
    @user = report_generation.requested_by
    @academy = report_generation.academy
    
    mail(
      to: @user.email,
      subject: "Report Generation Failed: #{@report.name}",
      template_path: 'report_mailer',
      template_name: 'generation_failed'
    )
  end
  
  def scheduled_report_ready(report_generation, subscriber)
    @report_generation = report_generation
    @report = report_generation.report
    @subscriber = subscriber
    @academy = report_generation.academy
    
    # Attach report file if subscriber preferences allow
    if subscriber.include_attachment && report_generation.file_size < 10.megabytes
      attachments["#{@report.name}_#{Date.current.strftime('%Y%m%d')}.pdf"] = {
        mime_type: 'application/pdf',
        content: download_file_content(report_generation.file_url)
      }
    end
    
    mail(
      to: subscriber.email,
      subject: "Scheduled Report: #{@report.name}",
      template_path: 'report_mailer',
      template_name: 'scheduled_report_ready'
    )
  end
  
  private
  
  def download_file_content(url)
    # Download file content from cloud storage
    # Return binary content for attachment
  end
end
```text

## Service Classes

### ReportGenerationService

```ruby
class ReportGenerationService < BaseService
  def initialize(report_generation_id:)
    @report_generation = ReportGeneration.find(report_generation_id)
    @report = @report_generation.report
    @user = @report_generation.requested_by
  end
  
  def call
    @report_generation.update!(status: :processing, started_at: Time.current)
    
    begin
      # Generate report content based on type
      report_data = generate_report_content
      
      # Create PDF/Excel file
      file_path = create_report_file(report_data)
      
      # Upload to cloud storage
      file_url = upload_to_storage(file_path)
      
      # Update generation record with success
      @report_generation.update!(
        status: :completed,
        completed_at: Time.current,
        file_url: file_url,
        file_size: File.size(file_path)
      )
      
      # Queue email notification job
      ReportDeliveryJob.perform_async(@report_generation.id)
      
      # Clean up temporary file
      File.delete(file_path) if File.exist?(file_path)
      
      @report_generation
    rescue => e
      @report_generation.update!(
        status: :failed,
        completed_at: Time.current,
        error_message: e.message
      )
      
      # Queue failure notification email
      ReportFailureNotificationJob.perform_async(@report_generation.id)
      
      raise e
    end
  end
  
  private
  
  def generate_report_content
    case @report.report_type
    when 'churn_analysis'
      ChurnAnalysisService.new(
        academy: @report.academy,
        date_range_start: @report_generation.date_range_start,
        date_range_end: @report_generation.date_range_end,
        filters: @report_generation.parameters
      ).analyze_churn_patterns
    when 'growth_tracking'
      GrowthTrackingService.new(
        academy: @report.academy,
        date_range_start: @report_generation.date_range_start,
        date_range_end: @report_generation.date_range_end
      ).analyze_growth_metrics
    when 'retention_analysis'
      RetentionAnalysisService.new(
        academy: @report.academy,
        cohort_period: @report_generation.parameters['cohort_period'] || 'monthly'
      ).cohort_analysis
    when 'lifetime_value'
      CustomerLifetimeValueService.new(
        academy: @report.academy,
        calculation_method: @report_generation.parameters['calculation_method'] || 'predictive'
      ).calculate_clv_metrics
    when 'acquisition_analysis'
      generate_acquisition_analysis_report
    when 'seasonal_trends'
      generate_seasonal_trends_report
    when 'competitive_analysis'
      generate_competitive_analysis_report
    when 'program_effectiveness'
      generate_program_effectiveness_report
    when 'operational_efficiency'
      generate_operational_efficiency_report
    when 'parent_satisfaction'
      generate_parent_satisfaction_report
    when 'financial_summary'
      generate_financial_summary_report
    when 'player_development'
      generate_player_development_report
    when 'attendance_analysis'
      generate_attendance_analysis_report
    when 'coaching_effectiveness'
      generate_coaching_effectiveness_report
    else
      raise ArgumentError, "Unknown report type: #{@report.report_type}"
    end
  end
  
  def create_report_file(data)
    # Generate PDF or Excel file from data
    # Return temporary file path
  end
  
  def upload_to_storage(file_path)
    # Upload to AWS S3 or similar
    # Return public URL with expiration
  end
  
  private
  
  def generate_churn_analysis_report
    # Calculate player churn rates by time period, age group, program
    # Identify at-risk players using predictive modeling
    # Analyze churn reasons and patterns
    # Generate retention improvement recommendations
  end
  
  def generate_growth_tracking_report
    # Track player enrollment growth over time
    # Analyze growth by program, age group, location
    # Compare actual vs projected growth
    # Identify growth opportunities and bottlenecks
  end
  
  def generate_retention_analysis_report
    # Calculate retention rates by cohort
    # Analyze retention factors and success patterns
    # Track player lifecycle progression
    # Identify high-value retention strategies
  end
  
  def generate_lifetime_value_report
    # Calculate Customer Lifetime Value (CLV) for players
    # Segment players by value and potential
    # Analyze revenue per player over time
    # Forecast future value and profitability
  end
  
  def generate_acquisition_analysis_report  
    # Analyze player acquisition channels and costs
    # Track conversion rates from inquiry to enrollment
    # Calculate Customer Acquisition Cost (CAC)
    # Evaluate marketing campaign effectiveness
  end
  
  def generate_seasonal_trends_report
    # Analyze seasonal patterns in enrollment and attendance
    # Track program popularity by season
    # Identify optimal scheduling and resource allocation
    # Forecast seasonal demand and capacity needs
  end
  
  def generate_competitive_analysis_report
    # Compare academy performance vs market benchmarks
    # Analyze pricing strategies and market positioning
    # Track market share and competitive advantages
    # Identify differentiation opportunities
  end
  
  def generate_program_effectiveness_report
    # Measure program success rates and outcomes
    # Analyze program profitability and resource utilization
    # Compare program performance across locations
    # Identify program optimization opportunities
  end
  
  def generate_operational_efficiency_report
    # Analyze staff utilization and productivity
    # Track facility usage and optimization
    # Measure cost per player and operational metrics
    # Identify efficiency improvement opportunities
  end
  
  def generate_parent_satisfaction_report
    # Analyze parent satisfaction scores and feedback
    # Track Net Promoter Score (NPS) trends
    # Identify satisfaction drivers and pain points
    # Generate parent experience improvement recommendations
  end
  
  def generate_financial_report
    # Financial report generation logic
  end
  
  def generate_player_analytics_report
    # Player analytics report generation logic
  end
  
  # ... other existing report type generators
end
```text

### FinancialAnalyticsService

```ruby
class FinancialAnalyticsService < BaseService
  def initialize(academy:, date_range_start:, date_range_end:)
    @academy = academy
    @date_range_start = date_range_start
    @date_range_end = date_range_end
  end
  
  def revenue_analysis
    # Calculate total revenue by category
    # Compare with previous periods
    # Identify trends and patterns
  end
  
  def expense_analysis
    # Calculate total expenses by category
    # Track budget vs. actual spending
    # Identify cost optimization opportunities
  end
  
  def profit_loss_statement
    # Generate comprehensive P&L statement
    # Include all revenue and expense categories
    # Calculate net profit/loss
  end
  
  def cash_flow_projection
    # Project future cash flows based on historical data
    # Account for seasonal variations
    # Include upcoming known income/expenses
  end
end
```text

### PlayerAnalyticsService

```ruby
class PlayerAnalyticsService < BaseService
  def initialize(academy:, player_ids: nil, date_range_start:, date_range_end:)
    @academy = academy
    @player_ids = player_ids
    @date_range_start = date_range_start
    @date_range_end = date_range_end
  end
  
  def development_tracking
    # Track individual player skill improvements
    # Calculate development rates and trends
    # Identify players needing additional support
  end
  
  def attendance_analysis
    # Calculate attendance rates by player
    # Identify attendance patterns and issues
    # Generate attendance improvement recommendations
  end
  
  def performance_metrics
    # Aggregate performance data from various sources
    # Calculate composite performance scores
    # Rank players within categories/positions
  end
  
  def retention_analysis
    # Analyze player retention rates
    # Identify factors contributing to dropouts
    # Suggest retention improvement strategies
  end
end
```text

### ChurnAnalysisService

```ruby
class ChurnAnalysisService < BaseService
  def initialize(academy:, date_range_start:, date_range_end:, filters: {})
    @academy = academy
    @date_range_start = date_range_start
    @date_range_end = date_range_end
    @filters = filters
  end
  
  def analyze_churn_patterns
    {
      churn_rate: calculate_churn_rate,
      churn_by_segment: analyze_churn_by_segment,
      churn_reasons: analyze_churn_reasons,
      at_risk_players: identify_at_risk_players,
      retention_strategies: recommend_retention_strategies
    }
  end
  
  private
  
  def calculate_churn_rate
    # Calculate overall and segmented churn rates
    # Track churn trends over time
    # Compare with industry benchmarks
  end
  
  def identify_at_risk_players
    # Use predictive modeling to identify players at risk
    # Score players based on behavior patterns
    # Recommend intervention strategies
  end
  
  def recommend_retention_strategies
    # Analyze successful retention tactics
    # Recommend personalized retention approaches
    # Calculate ROI of retention investments
  end
end
```text

### GrowthTrackingService

```ruby
class GrowthTrackingService < BaseService
  def initialize(academy:, date_range_start:, date_range_end:)
    @academy = academy
    @date_range_start = date_range_start
    @date_range_end = date_range_end
  end
  
  def analyze_growth_metrics
    {
      enrollment_trends: calculate_enrollment_trends,
      growth_by_program: analyze_program_growth,
      acquisition_performance: analyze_acquisition_channels,
      capacity_utilization: calculate_capacity_metrics,
      growth_projections: forecast_growth
    }
  end
  
  private
  
  def calculate_enrollment_trends
    # Track enrollment changes over time
    # Calculate growth rates and momentum
    # Identify seasonal patterns
  end
  
  def analyze_acquisition_channels
    # Evaluate channel performance and ROI
    # Track conversion rates by channel
    # Optimize acquisition spend allocation
  end
  
  def forecast_growth
    # Use historical data to project future growth
    # Account for seasonality and market factors
    # Provide scenario-based projections
  end
end
```text

### RetentionAnalysisService

```ruby
class RetentionAnalysisService < BaseService
  def initialize(academy:, cohort_period: 'monthly')
    @academy = academy
    @cohort_period = cohort_period
  end
  
  def cohort_analysis
    {
      retention_curves: build_retention_curves,
      cohort_comparison: compare_cohorts,
      retention_factors: analyze_retention_factors,
      lifecycle_analysis: analyze_player_lifecycle
    }
  end
  
  private
  
  def build_retention_curves
    # Create retention curves for different cohorts
    # Track retention at 30, 90, 180 days, 1 year
    # Identify retention patterns and trends
  end
  
  def analyze_retention_factors
    # Identify factors that correlate with high retention
    # Measure impact of different programs and initiatives
    # Provide actionable retention insights
  end
end
```text

### CustomerLifetimeValueService

```ruby
class CustomerLifetimeValueService < BaseService
  def initialize(academy:, calculation_method: 'predictive')
    @academy = academy
    @calculation_method = calculation_method
  end
  
  def calculate_clv_metrics
    {
      average_clv: calculate_average_clv,
      clv_by_segment: segment_clv_analysis,
      value_drivers: identify_value_drivers,
      profitability_analysis: analyze_profitability
    }
  end
  
  private
  
  def calculate_average_clv
    # Calculate CLV using historical and predictive methods
    # Account for churn probability and revenue trends
    # Segment by player characteristics
  end
  
  def identify_value_drivers
    # Analyze factors that increase CLV
    # Measure impact of programs on lifetime value
    # Provide CLV optimization recommendations
  end
end
```text

### BusinessIntelligenceService

```ruby
class BusinessIntelligenceService < BaseService
  def initialize(academy:, analysis_type:, date_range_start:, date_range_end:)
    @academy = academy
    @analysis_type = analysis_type
    @date_range_start = date_range_start
    @date_range_end = date_range_end
  end
  
  def cross_academy_benchmarking
    # Compare academy performance against industry benchmarks
    # Analyze key performance indicators
    # Identify areas for improvement
  end
  
  def predictive_modeling
    # Use machine learning for player retention prediction
    # Forecast enrollment trends
    # Predict optimal pricing strategies
  end
  
  def advanced_segmentation
    # Segment players by behavior, value, and potential
    # Create targeted retention strategies
    # Optimize marketing campaigns
  end
  
  def operational_efficiency_analysis
    # Analyze staff productivity and utilization
    # Optimize facility usage and scheduling
    # Identify cost reduction opportunities
  end
  
  def parent_satisfaction_analysis
    # Track parent satisfaction metrics and NPS
    # Analyze feedback themes and sentiment
    # Recommend satisfaction improvement initiatives
  end
end
```text

### AITrainingGenerationService

```ruby
class AITrainingGenerationService < BaseService
  def initialize(player:, parameters: {})
    @player = player
    @parameters = parameters
  end
  
  def generate_personalized_training
    # Analyze player data and generate customized exercises
    # Consider skill level, position, recent assessments
    # Adapt difficulty based on age and development stage
    # Include equipment and space constraints
    # Generate instructional content and progressions
  end
  
  def analyze_player_profile
    # Combine multiple data sources for comprehensive analysis
    # Recent skill assessments and coach feedback
    # Training attendance and performance trends
    # Position-specific requirements and development goals
    # Previous home training completion and feedback
  end
  
  def select_appropriate_exercises
    # Choose exercises based on focus areas and constraints
    # Consider available equipment and space requirements
    # Match difficulty to player's current ability level
    # Ensure age-appropriate duration and complexity
    # Include progression options for adaptive difficulty
  end
  
  def generate_coaching_content
    # Create detailed instructions and coaching tips
    # Generate video recommendations and visual aids
    # Provide parent guidance and supervision notes
    # Include safety considerations and equipment setup
    # Add motivational elements and progress tracking
  end
  
  private
  
  def player_skill_matrix
    # Calculate current skill levels across all areas
    # Weight recent assessments more heavily
    # Consider position-specific skill importance
    # Account for age-appropriate development expectations
  end
  
  def training_history_analysis
    # Analyze previous home training sessions
    # Identify patterns in completion rates and feedback
    # Adjust difficulty based on historical performance
    # Consider parent and player preferences
  end
  
  def exercise_recommendation_engine
    # Use ML algorithms to recommend optimal exercises
    # Consider exercise effectiveness for specific skills
    # Factor in player engagement and enjoyment data
    # Adapt based on seasonal training focus
  end
end
```text

## API Endpoints

### Reports API

#### List Available Reports

```text
GET /api/v1/{academy_slug}/reports
```text

**Query Parameters:**

- `type` - Filter by report type
- `category` - Filter by report category
- `scheduled` - Filter by scheduled reports (true/false)

#### Generate Report (Async)

```text
POST /api/v1/{academy_slug}/reports/{slug}/generate
```text

**Request Body:**

```json
{
  "date_range_start": "2025-01-01",
  "date_range_end": "2025-10-13",
  "format": "pdf",
  "priority": 1,
  "email_when_ready": true,
  "parameters": {
    "include_charts": true,
    "currency": "USD",
    "team_ids": ["team-1", "team-2"]
  }
}
```text

**Response:**

```json
{
  "data": {
    "generation_slug": "gen-abc123",
    "status": "queued",
    "estimated_completion_time": "2025-10-13T14:35:00Z",
    "estimated_size_mb": 2.5,
    "priority": 1,
    "queue_position": 3,
    "message": "Report generation queued successfully. You will receive an email when ready."
  },
  "links": {
    "status": "/api/v1/barcelona-academy/reports/financial-summary/generations/gen-abc123",
    "cancel": "/api/v1/barcelona-academy/reports/financial-summary/generations/gen-abc123/cancel"
  }
}
```text

#### Get Report Generation Status

```text
GET /api/v1/{academy_slug}/reports/{slug}/generations/{generation_slug}
```text

**Response:**

```json
{
  "data": {
    "generation_slug": "gen-abc123",
    "status": "processing",
    "progress_percentage": 65,
    "started_at": "2025-10-13T14:30:00Z",
    "estimated_completion_time": "2025-10-13T14:35:00Z",
    "current_step": "Generating charts and visualizations",
    "file_size_mb": 2.3,
    "download_count": 0,
    "expires_at": "2025-11-13T14:35:00Z"
  },
  "links": {
    "download": "/api/v1/barcelona-academy/reports/financial-summary/generations/gen-abc123/download",
    "cancel": "/api/v1/barcelona-academy/reports/financial-summary/generations/gen-abc123/cancel"
  }
}
```text

#### Cancel Report Generation

```text
DELETE /api/v1/{academy_slug}/reports/{slug}/generations/{generation_slug}/cancel
```text

**Response:**

```json
{
  "data": {
    "generation_slug": "gen-abc123",
    "status": "cancelled",
    "message": "Report generation cancelled successfully"
  }
}
```text

#### Download Generated Report

```text
GET /api/v1/{academy_slug}/reports/{slug}/generations/{generation_slug}/download
```text

**Response:**

- **Success (302)**: Redirects to signed cloud storage URL
- **Pending (202)**: Report still generating
- **Expired (410)**: Report file has expired

#### List Report Generations

```text
GET /api/v1/{academy_slug}/reports/{slug}/generations
```text

**Query Parameters:**

- `status` - Filter by status (queued, processing, completed, failed, cancelled)
- `limit` - Number of results (default: 20, max: 100)
- `offset` - Pagination offset

**Response:**

```json
{
  "data": [
    {
      "generation_slug": "gen-abc123",
      "status": "completed",
      "requested_at": "2025-10-13T14:30:00Z",
      "completed_at": "2025-10-13T14:35:30Z",
      "processing_time_minutes": 5.5,
      "file_size_mb": 2.3,
      "download_count": 12,
      "expires_at": "2025-11-13T14:35:00Z",
      "is_scheduled": false
    }
  ],
  "meta": {
    "total": 45,
    "limit": 20,
    "offset": 0,
    "has_more": true
  }
}
```text

#### Bulk Report Generation

```text
POST /api/v1/{academy_slug}/reports/bulk-generate
```text

**Request Body:**

```json
{
  "reports": [
    {
      "report_slug": "financial-summary",
      "date_range_start": "2025-01-01",
      "date_range_end": "2025-10-13",
      "format": "pdf"
    },
    {
      "report_slug": "player-development",
      "date_range_start": "2025-01-01",
      "date_range_end": "2025-10-13",
      "format": "excel"
    }
  ],
  "priority": 2,
  "email_when_ready": true
}
```text

**Response:**

```json
{
  "data": {
    "bulk_generation_id": "bulk-xyz789",
    "queued_reports": 2,
    "estimated_total_time_minutes": 8,
    "generations": [
      {
        "report_slug": "financial-summary",
        "generation_slug": "gen-abc123",
        "status": "queued"
      },
      {
        "report_slug": "player-development", 
        "generation_slug": "gen-def456",
        "status": "queued"
      }
    ]
  }
}
```text

### Financial Analytics API

#### Revenue Summary

```text
GET /api/v1/{academy_slug}/analytics/revenue
```text

**Query Parameters:**

- `start_date`, `end_date` - Date range
- `category` - Filter by revenue category
- `compare_previous` - Include previous period comparison (true/false)

#### Expense Analysis

```text
GET /api/v1/{academy_slug}/analytics/expenses
```text

#### Profit & Loss Statement

```text
GET /api/v1/{academy_slug}/analytics/profit-loss
```text

### Churn & Growth Analytics API

#### Player Churn Analysis

```text
GET /api/v1/{academy_slug}/analytics/churn-analysis
```text

**Query Parameters:**

- `period` - Analysis period (monthly, quarterly, yearly)
- `start_date` - Analysis start date
- `end_date` - Analysis end date
- `age_group` - Filter by specific age group
- `program_type` - Filter by program type

**Response:**

```json
{
  "data": {
    "summary": {
      "total_players_start": 120,
      "total_players_end": 108,
      "churned_players": 18,
      "new_players": 6,
      "churn_rate": 15.0,
      "net_growth": -10.0
    },
    "churn_by_period": [
      {
        "month": "2025-09",
        "churned": 6,
        "total_active": 115,
        "churn_rate": 5.2
      }
    ],
    "churn_by_age_group": [
      {
        "age_group": "U8",
        "churned": 4,
        "total": 25,
        "churn_rate": 16.0,
        "primary_reasons": ["schedule_conflicts", "cost"]
      }
    ],
    "at_risk_players": [
      {
        "player_slug": "player-123",
        "risk_score": 0.85,
        "risk_factors": ["low_attendance", "parent_complaints"],
        "recommendations": ["schedule_flexibility", "parent_meeting"]
      }
    ],
    "retention_strategies": [
      {
        "strategy": "flexible_scheduling",
        "impact_score": 0.72,
        "implementation_cost": "low"
      }
    ]
  }
}
```text

#### Growth Tracking Report

```text
GET /api/v1/{academy_slug}/analytics/growth-tracking
```text

**Response:**

```json
{
  "data": {
    "growth_summary": {
      "current_enrollment": 108,
      "growth_rate_monthly": 2.5,
      "growth_rate_yearly": 18.2,
      "projected_enrollment_6m": 125,
      "capacity_utilization": 72.0
    },
    "enrollment_trends": [
      {
        "month": "2025-10",
        "new_enrollments": 8,
        "total_enrollment": 108,
        "growth_rate": 2.5
      }
    ],
    "growth_by_program": [
      {
        "program": "Competitive U12",
        "current_players": 25,
        "growth_rate": 15.6,
        "waiting_list": 3
      }
    ],
    "acquisition_channels": [
      {
        "channel": "referral",
        "new_players": 12,
        "conversion_rate": 85.0,
        "acquisition_cost": 25.50
      }
    ],
    "seasonal_patterns": {
      "peak_months": ["August", "September"],
      "low_months": ["December", "January"],
      "seasonal_variance": 32.0
    }
  }
}
```text

#### Player Retention Analysis

```text
GET /api/v1/{academy_slug}/analytics/retention-analysis
```text

**Response:**

```json
{
  "data": {
    "retention_rates": {
      "30_day": 92.5,
      "90_day": 85.0,
      "180_day": 78.2,
      "1_year": 72.1
    },
    "cohort_analysis": [
      {
        "cohort": "2025-Q1",
        "initial_size": 25,
        "retained_3m": 22,
        "retained_6m": 20,
        "retained_1y": 18,
        "retention_rate": 72.0
      }
    ],
    "retention_factors": [
      {
        "factor": "parent_involvement",
        "correlation": 0.78,
        "impact": "high"
      },
      {
        "factor": "coach_relationship",
        "correlation": 0.65,
        "impact": "high"
      }
    ],
    "lifecycle_progression": [
      {
        "stage": "beginner",
        "avg_duration_months": 6,
        "progression_rate": 85.0,
        "dropout_reasons": ["difficulty", "time_commitment"]
      }
    ]
  }
}
```text

#### Customer Lifetime Value Report

```text
GET /api/v1/{academy_slug}/analytics/lifetime-value
```text

#### Acquisition Analysis

```text
GET /api/v1/{academy_slug}/analytics/acquisition-analysis
```text

**Response:**

```json
{
  "data": {
    "acquisition_summary": {
      "total_acquisitions": 45,
      "avg_acquisition_cost": 32.75,
      "conversion_rate": 22.5,
      "best_performing_channel": "referral"
    },
    "channels": [
      {
        "channel": "referral",
        "acquisitions": 18,
        "cost": 15.50,
        "conversion_rate": 45.0,
        "roi": 290.5
      },
      {
        "channel": "social_media",
        "acquisitions": 12,
        "cost": 45.25,
        "conversion_rate": 18.0,
        "roi": 145.2
      }
    ],
    "conversion_funnel": [
      {
        "stage": "inquiry",
        "count": 200,
        "conversion_rate": 100.0
      },
      {
        "stage": "trial_session",
        "count": 120,
        "conversion_rate": 60.0
      },
      {
        "stage": "enrollment",
        "count": 45,
        "conversion_rate": 22.5
      }
    ]
  }
}
```text

### Business Intelligence API

#### Seasonal Trends Analysis

```text
GET /api/v1/{academy_slug}/analytics/seasonal-trends
```text

#### Competitive Analysis

```text
GET /api/v1/{academy_slug}/analytics/competitive-analysis
```text

#### Program Effectiveness Report

```text
GET /api/v1/{academy_slug}/analytics/program-effectiveness
```text

#### Operational Efficiency Report

```text
GET /api/v1/{academy_slug}/analytics/operational-efficiency
```text

**Response:**

```json
{
  "data": {
    "efficiency_metrics": {
      "coach_utilization": 78.5,
      "facility_utilization": 82.3,
      "cost_per_player": 125.75,
      "revenue_per_hour": 45.25
    },
    "staff_productivity": [
      {
        "coach_slug": "coach-123",
        "hours_scheduled": 40,
        "hours_utilized": 38,
        "player_satisfaction": 4.6,
        "efficiency_score": 92.5
      }
    ],
    "facility_optimization": {
      "peak_hours": ["16:00-18:00", "18:00-20:00"],
      "underutilized_periods": ["10:00-14:00"],
      "capacity_recommendations": [
        "Add morning programs for preschoolers",
        "Offer adult leagues during underutilized hours"
      ]
    },
    "cost_optimization": [
      {
        "category": "equipment",
        "current_cost": 2500.0,
        "optimized_cost": 2100.0,
        "savings_potential": 400.0
      }
    ]
  }
}
```text

#### Parent Satisfaction Report

```text
GET /api/v1/{academy_slug}/analytics/parent-satisfaction
```text

**Response:**

```json
{
  "data": {
    "satisfaction_metrics": {
      "overall_satisfaction": 4.3,
      "net_promoter_score": 72,
      "retention_likelihood": 85.2,
      "recommendation_rate": 78.5
    },
    "satisfaction_by_category": [
      {
        "category": "coaching_quality",
        "score": 4.5,
        "improvement_trend": "stable"
      },
      {
        "category": "communication",
        "score": 4.1,
        "improvement_trend": "improving"
      },
      {
        "category": "value_for_money",
        "score": 3.9,
        "improvement_trend": "declining"
      }
    ],
    "feedback_themes": [
      {
        "theme": "flexible_scheduling",
        "mentions": 25,
        "sentiment": "positive",
        "priority": "high"
      }
    ],
    "improvement_areas": [
      {
        "area": "communication_frequency",
        "impact_score": 0.65,
        "implementation_effort": "medium"
      }
    ]
  }
}
```text

### Player Analytics API

#### Player Development Report

```text
GET /api/v1/{academy_slug}/analytics/player-development
```text

#### Attendance Analytics

```text
GET /api/v1/{academy_slug}/analytics/attendance
```text

### AI Training Analytics API

#### Generate AI Training Session

```text
POST /api/v1/{academy_slug}/players/{player_slug}/ai-training/generate
```text

#### Get AI Training History

```text
GET /api/v1/{academy_slug}/players/{player_slug}/ai-training/sessions
```text

#### AI Training Analytics

```text
GET /api/v1/{academy_slug}/players/{player_slug}/ai-training/analytics
```text

**Response:**

```json
{
  "data": {
    "player_slug": "player-123",
    "total_sessions_generated": 25,
    "total_sessions_completed": 20,
    "completion_rate": 80.0,
    "average_enjoyment_rating": 4.2,
    "average_difficulty_rating": 3.1,
    "most_improved_skills": ["ball_control", "first_touch"],
    "skill_progress": {
      "ball_control": {
        "initial_rating": 6.0,
        "current_rating": 7.5,
        "improvement": 1.5,
        "sessions_focused": 12
      },
      "shooting": {
        "initial_rating": 5.5,
        "current_rating": 6.8,
        "improvement": 1.3,
        "sessions_focused": 8
      }
    },
    "preferred_training_types": [
      {
        "category": "ball_control",
        "frequency": 15,
        "avg_enjoyment": 4.5
      }
    ],
    "home_training_insights": {
      "optimal_session_duration": 25,
      "best_training_times": ["afternoon", "early_evening"],
      "parent_involvement_impact": 0.8,
      "equipment_preferences": ["ball", "cones", "wall"]
    },
    "ai_adaptation_metrics": {
      "difficulty_adjustments": 8,
      "exercise_substitutions": 12,
      "personalization_accuracy": 87.5
    }
  }
}
```text

#### Bulk AI Training Analytics

```text
GET /api/v1/{academy_slug}/analytics/ai-training
```text

**Query Parameters:**

- `player_ids` - Specific players (comma-separated)
- `start_date` - Analysis start date
- `end_date` - Analysis end date
- `min_sessions` - Minimum sessions for inclusion

**Response:**

```json
{
  "data": {
    "summary": {
      "total_players_using_ai": 85,
      "total_sessions_generated": 1250,
      "total_sessions_completed": 980,
      "overall_completion_rate": 78.4,
      "average_enjoyment_rating": 4.1,
      "most_popular_focus_areas": [
        "ball_control",
        "shooting",
        "passing"
      ]
    },
    "player_analytics": [
      {
        "player_slug": "player-123",
        "sessions_completed": 20,
        "completion_rate": 80.0,
        "avg_enjoyment": 4.2,
        "skill_improvements": {
          "ball_control": 1.5,
          "shooting": 1.3
        }
      }
    ],
    "trends": {
      "completion_rate_trend": "improving",
      "enjoyment_trend": "stable",
      "difficulty_adaptation": "effective"
    },
    "recommendations": [
      "Increase shooting exercise variety",
      "Add more parent-child interactive exercises",
      "Develop rainy day indoor alternatives"
    ]
  }
}
```text

### System-Wide Analytics API (Multi-Academy)

#### Academy Benchmarking

```text
GET /api/v1/system/analytics/benchmarking
```text

#### Growth Projections

```text
GET /api/v1/system/analytics/growth-projections
```text

#### AI Training System Analytics

```text
GET /api/v1/system/analytics/ai-training
```text

**Response:**

```json
{
  "data": {
    "global_metrics": {
      "total_academies_using_ai": 15,
      "total_players_active": 1250,
      "total_sessions_generated": 25000,
      "overall_completion_rate": 74.2,
      "average_skill_improvement": 1.8
    },
    "academy_comparison": [
      {
        "academy_slug": "barcelona-academy",
        "players_using_ai": 85,
        "completion_rate": 82.1,
        "avg_improvement": 2.1,
        "parent_engagement": 91.5
      }
    ],
    "ai_system_performance": {
      "personalization_accuracy": 89.2,
      "exercise_effectiveness": 4.3,
      "adaptation_success_rate": 85.7,
      "user_satisfaction": 4.4
    }
  }
}
```text

## Background Jobs

### ScheduledReportGenerationJob

```ruby
class ScheduledReportGenerationJob < ApplicationJob
  def perform
    # Find all scheduled reports due for generation
    # Generate reports in background
    # Send notifications to subscribers
    # Clean up old report files
  end
end
```text

### ReportCleanupJob

```ruby
class ReportCleanupJob < ApplicationJob
  def perform
    # Delete expired report files from cloud storage
    # Clean up old report generation records
    # Maintain storage optimization
  end
end
```text

### MetricsCalculationJob

```ruby
class MetricsCalculationJob < ApplicationJob
  def perform(academy_id = nil)
    # Calculate and store player metrics
    # Update financial summaries
    # Generate performance indicators
    # Schedule next calculation
  end
end
```text

## Dashboard & Visualization

### Executive Dashboard Components

- **Financial Overview**: Revenue, expenses, profit trends
- **Academy Health**: Enrollment, retention, satisfaction scores
- **Performance Metrics**: Key indicators and alerts
- **Upcoming Activities**: Scheduled reports, important dates
- **Quick Actions**: Generate common reports, view analytics

### Interactive Charts & Graphs

- **Revenue Trends**: Monthly/quarterly revenue visualization
- **Player Growth**: Enrollment and development charts
- **Financial Breakdown**: Pie charts for income/expense categories
- **Comparative Analysis**: Multi-academy performance comparison
- **Goal Tracking**: Progress towards financial and operational goals

### Mobile Dashboard

- **Key Metrics Summary**: Essential KPIs on mobile
- **Quick Report Access**: Common reports optimized for mobile
- **Alerts & Notifications**: Important updates and warnings
- **Offline Capability**: Cached data for basic reporting

## Integration Points

### With Asset Management

- Asset depreciation in financial reports
- Equipment maintenance costs analysis
- Asset utilization efficiency metrics
- ROI calculations for equipment purchases

### With Player Management

- Player lifetime value calculations
- Registration fee tracking and analysis
- Player development cost analysis
- Retention and recruitment effectiveness

### With Training Management

- Training cost per player calculations
- Coach effectiveness metrics
- Facility utilization analysis
- Training outcome measurements

### With Academy Management

- Multi-academy consolidated reporting
- Academy performance benchmarking
- Operational efficiency comparisons
- Best practices identification

## Export & Sharing Capabilities

### Export Formats

- **PDF Reports**: Professional formatted reports
- **Excel Spreadsheets**: Data with calculations and charts
- **CSV Data**: Raw data for external analysis
- **JSON API**: Programmatic access to report data
- **PowerBI/Tableau**: Integration with BI tools

### Sharing Options

- **Email Distribution**: Automated report delivery
- **Dashboard Sharing**: Shared dashboard access
- **API Access**: External system integration
- **Print Optimization**: Physical report printing
- **Mobile Sharing**: Optimized mobile report viewing

## Implementation Priority

### Phase 1: Core Financial Reporting

- Basic income/expense tracking
- Simple financial reports (P&L, revenue summary)
- Report generation and download

### Phase 2: Player Analytics

- Player development tracking
- Attendance analysis
- Performance metrics calculation

### Phase 3: Advanced Analytics

- Business intelligence dashboards
- Predictive analytics
- Multi-academy benchmarking
- Automated insights and recommendations

### Phase 4: Integration & Automation

- Real-time dashboard updates
- Automated report scheduling
- Advanced visualizations
- Mobile analytics app

This comprehensive reporting system provides academies with the insights needed to make data-driven decisions while maintaining scalability for multi-academy organizations.

## Performance Configuration & Job Management

### Sidekiq Queue Configuration

```ruby
# config/initializers/sidekiq.rb
Sidekiq.configure_server do |config|
  config.redis = { url: ENV['REDIS_URL'] }
  
  # Report generation queues with priorities
  config.queues = %w[critical_reports reports scheduled_reports notifications default]
  
  # Concurrency settings for report generation
  config.concurrency = ENV.fetch('SIDEKIQ_CONCURRENCY', 25).to_i
end

Sidekiq.configure_client do |config|
  config.redis = { url: ENV['REDIS_URL'] }
end

# Custom queue prioritization
Sidekiq::Queue.new('critical_reports').priority = 10
Sidekiq::Queue.new('reports').priority = 5  
Sidekiq::Queue.new('scheduled_reports').priority = 3
Sidekiq::Queue.new('notifications').priority = 7
```text

### Report Generation Settings

```ruby
# config/application.rb
config.report_settings = {
  max_concurrent_generations: 5,
  max_file_size_mb: 50,
  default_expiry_days: 30,
  max_download_count: 100,
  chunk_size_for_large_reports: 10000,
  enable_report_caching: true,
  cache_duration_hours: 24,
  enable_compression: true,
  notification_retry_attempts: 3
}

# Memory optimization for large reports
config.report_memory_limits = {
  max_memory_per_job_mb: 512,
  enable_gc_after_generation: true,
  stream_large_datasets: true
}
```text

### Email Configuration for Reports

```ruby
# config/environments/production.rb
config.action_mailer.delivery_method = :smtp
config.action_mailer.smtp_settings = {
  address: ENV['SMTP_HOST'],
  port: ENV['SMTP_PORT'],
  user_name: ENV['SMTP_USERNAME'],
  password: ENV['SMTP_PASSWORD'],
  authentication: 'plain',
  enable_starttls_auto: true
}

# Report-specific email settings
config.report_email_settings = {
  max_attachment_size_mb: 10,
  fallback_to_download_link: true,
  include_preview_in_email: true,
  batch_size_for_bulk_notifications: 50
}
```text

### Cloud Storage Configuration

```ruby
# config/storage.yml
report_storage:
  service: S3
  access_key_id: <%= ENV['AWS_ACCESS_KEY_ID'] %>
  secret_access_key: <%= ENV['AWS_SECRET_ACCESS_KEY'] %>
  region: <%= ENV['AWS_REGION'] %>
  bucket: <%= ENV['REPORTS_S3_BUCKET'] %>
  public: false
  
report_storage_cdn:
  service: S3
  access_key_id: <%= ENV['AWS_ACCESS_KEY_ID'] %>
  secret_access_key: <%= ENV['AWS_SECRET_ACCESS_KEY'] %>
  region: <%= ENV['AWS_REGION'] %>
  bucket: <%= ENV['REPORTS_CDN_BUCKET'] %>
  public: true
```text

### Performance Monitoring

```ruby
# app/services/report_performance_monitor.rb
class ReportPerformanceMonitor
  def self.track_generation(report_generation)
    StatsD.histogram('report.generation.time', report_generation.processing_time)
    StatsD.histogram('report.generation.size', report_generation.file_size)
    StatsD.increment("report.generation.#{report_generation.status}")
    StatsD.increment("report.type.#{report_generation.report.report_type}")
  end
  
  def self.track_queue_metrics
    StatsD.gauge('report.queue.size', Sidekiq::Queue.new('reports').size)
    StatsD.gauge('report.queue.latency', Sidekiq::Queue.new('reports').latency)
  end
end
```text

## Email Templates

### Report Completion Email

```erb
<!-- app/views/report_mailer/generation_completed.html.erb -->
<h2>Your report is ready!</h2>

<p>Hello <%= @user.first_name %>,</p>

<p>Your <strong><%= @report.name %></strong> report has been generated successfully.</p>

<div class="report-details">
  <h3>Report Details</h3>
  <ul>
    <li><strong>Academy:</strong> <%= @academy.name %></li>
    <li><strong>Date Range:</strong> <%= @report_generation.date_range_start.strftime('%B %d, %Y') %> - <%= @report_generation.date_range_end.strftime('%B %d, %Y') %></li>
    <li><strong>Generated:</strong> <%= @report_generation.completed_at.strftime('%B %d, %Y at %l:%M %p') %></li>
    <li><strong>File Size:</strong> <%= number_to_human_size(@report_generation.file_size) %></li>
    <li><strong>Format:</strong> <%= @report_generation.format.upcase %></li>
  </ul>
</div>

<% if @report_generation.file_size < 10.megabytes %>
  <p>Your report is attached to this email.</p>
<% else %>
  <p>Your report is too large to attach via email. Please download it using the link below:</p>
<% end %>

<div class="download-section">
  <a href="<%= @report_generation.file_url %>" class="download-button" style="background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 20px 0;">
    Download Report
  </a>
  <p><small>This download link expires on <%= @report_generation.expires_at.strftime('%B %d, %Y') %></small></p>
</div>

<p>If you have any questions about this report, please contact your academy administrator.</p>

<p>Best regards,<br>
The <%= @academy.name %> Team</p>
```text

### Report Generation Failed Email

```erb
<!-- app/views/report_mailer/generation_failed.html.erb -->
<h2>Report Generation Failed</h2>

<p>Hello <%= @user.first_name %>,</p>

<p>Unfortunately, we encountered an issue while generating your <strong><%= @report.name %></strong> report.</p>

<div class="error-details">
  <h3>Details</h3>
  <ul>
    <li><strong>Academy:</strong> <%= @academy.name %></li>
    <li><strong>Requested:</strong> <%= @report_generation.created_at.strftime('%B %d, %Y at %l:%M %p') %></li>
    <li><strong>Date Range:</strong> <%= @report_generation.date_range_start.strftime('%B %d, %Y') %> - <%= @report_generation.date_range_end.strftime('%B %d, %Y') %></li>
  </ul>
</div>

<p>Our technical team has been notified and will investigate the issue. You can try generating the report again, or contact support if the problem persists.</p>

<div class="retry-section">
  <a href="<%= reports_url(academy_slug: @academy.slug) %>" class="retry-button" style="background-color: #28a745; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 20px 0;">
    Try Again
  </a>
</div>

<p>We apologize for the inconvenience.</p>

<p>Best regards,<br>
The <%= @academy.name %> Team</p>
```text

## Implementation Benefits

### 🚀 **Performance Improvements**

- **Async Processing**: No more waiting for large reports to generate
- **Queue Management**: Prioritized report generation based on importance
- **Resource Optimization**: Better memory and CPU usage management
- **Scalability**: Handle multiple concurrent report generations

### 📧 **Enhanced User Experience**  

- **Email Notifications**: Users notified when reports are ready
- **Progress Tracking**: Real-time status updates on report generation
- **Cancellation Support**: Ability to cancel long-running reports
- **Bulk Operations**: Generate multiple reports simultaneously

### 🔧 **Operational Benefits**

- **Error Handling**: Comprehensive error tracking and notification
- **Monitoring**: Performance metrics and queue health monitoring
- **File Management**: Automatic cleanup and expiration of old reports
- **Retry Mechanisms**: Automatic retry of failed generations

### 📊 **Business Value**

- **Higher Throughput**: Generate more reports simultaneously
- **Better Resource Utilization**: Optimal use of server resources
- **Improved Reliability**: Robust error handling and recovery
- **Enhanced Analytics**: Better insights through comprehensive reporting

This asynchronous reporting system ensures optimal performance while providing a superior user experience through automated email delivery and comprehensive status tracking.

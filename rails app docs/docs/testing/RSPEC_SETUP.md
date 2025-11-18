# RSpec Configuration for Slice Generator# RSpec Configuration for Slice Generator

#

This file provides setup instructions and configuration for RSpec testing with the slice generator. Follow these steps to set up your testing environment.# This file provides setup instructions and configuration for RSpec testing

# with the slice generator. Follow these steps to set up your testing environment

## 1. Add RSpec to your Gemfile

# 1. Add RSpec to your Gemfile

Add these lines to your Gemfile:# Add these lines to your Gemfile:

```rubygroup :development, :test do

group :development, :test do  gem 'rspec-rails', '~> 6.1'

  gem 'rspec-rails', '~> 6.1'  gem 'factory_bot_rails', '~> 6.4'

  gem 'factory_bot_rails', '~> 6.4'  gem 'faker', '~> 3.2'

  gem 'faker', '~> 3.2'end

end

group :test do

group :test do  gem 'shoulda-matchers', '~> 5.3'

  gem 'shoulda-matchers', '~> 5.3'  gem 'database_cleaner-active_record', '~> 2.1'

  gem 'database_cleaner-active_record', '~> 2.1'  gem 'pundit-matchers', '~> 3.1'  # For policy testing

  gem 'pundit-matchers', '~> 3.1'  # For policy testing  gem 'webmock', '~> 3.18'         # For HTTP request mocking

  gem 'webmock', '~> 3.18'         # For HTTP request mocking  gem 'timecop', '~> 0.9'          # For time-based testing

  gem 'timecop', '~> 0.9'          # For time-based testingend

end

```# 2. Install RSpec

# Run these commands in your terminal:

## 2. Install RSpec

```bash

Run these commands in your terminal:bundle install

rails generate rspec:install

```bash```

bundle install

rails generate rspec:install## 3. Configure RSpec for Slice Structure

```

Update your `spec/rails_helper.rb` to include slice specs:

## 3. Configure RSpec for Slice Structure

```ruby

Update your `spec/rails_helper.rb` to include slice specs. The slice generator now places specs directly in the slice folders at `app/slices/*/specs/` instead of the traditional `spec/` directory.# Add this to load slice specs from app/slices/**/specs/**/*_spec.rb

RSpec.configure do |config|

Add this configuration to your `spec/rails_helper.rb`:  # This allows RSpec to find specs in the slice folders

  config.pattern = '{spec,app/slices}/**/*_spec.rb'

```rubyend

# Allow RSpec to find specs in slice folders```

RSpec.configure do |config|

  # This pattern tells RSpec to look for specs in both traditional spec/ and slice folders## 4. Configure RSpec

  config.pattern = '{spec,app/slices}/**/*_spec.rb'# Update your spec/rails_helper.rb with this configuration:

end

```RSpec.configure do |config|

  # Include Factory Bot methods

## 4. Main RSpec Configuration  config.include FactoryBot::Syntax::Methods



Update your `spec/rails_helper.rb` with this configuration:  # Include Pundit matchers

  config.include Pundit::Matchers

```ruby

RSpec.configure do |config|  # Configure database cleaner

  # Include Factory Bot methods  config.before(:suite) do

  config.include FactoryBot::Syntax::Methods    DatabaseCleaner.strategy = :transaction

    DatabaseCleaner.clean_with(:truncation)

  # Include Pundit matchers  end

  config.include Pundit::Matchers

  config.around(:each) do |example|

  # Configure database cleaner    DatabaseCleaner.cleaning do

  config.before(:suite) do      example.run

    DatabaseCleaner.strategy = :transaction    end

    DatabaseCleaner.clean_with(:truncation)  end

  end

  # Configure Shoulda Matchers

  config.around(:each) do |example|  config.include(Shoulda::Matchers::ActiveModel, type: :model)

    DatabaseCleaner.cleaning do  config.include(Shoulda::Matchers::ActiveRecord, type: :model)

      example.run

    end  # Use transactional fixtures

  end  config.use_transactional_fixtures = true



  # Configure Shoulda Matchers  # Infer spec type from file path

  config.include(Shoulda::Matchers::ActiveModel, type: :model)  config.infer_spec_type_from_file_location!

  config.include(Shoulda::Matchers::ActiveRecord, type: :model)

  # Filter lines from Rails gems in backtraces

  # Use transactional fixtures  config.filter_rails_from_backtrace!

  config.use_transactional_fixtures = trueend



  # Infer spec type from file path# 4. Configure Shoulda Matchers

  config.infer_spec_type_from_file_location!# Add this to spec/rails_helper.rb:



  # Filter lines from Rails gems in backtracesShoulda::Matchers.configure do |config|

  config.filter_rails_from_backtrace!  config.integrate do |with|

end    with.test_framework :rspec

```    with.library :rails

  end

## 5. Configure Shoulda Matchersend



Add this to `spec/rails_helper.rb`:# 5. Create factory files

# The slice generator expects these factories to exist:

```ruby

Shoulda::Matchers.configure do |config|# spec/factories/users.rb

  config.integrate do |with|FactoryBot.define do

    with.test_framework :rspec  factory :user do

    with.library :rails    email { Faker::Internet.email }

  end    name { Faker::Name.full_name }

end    

```    trait :admin do

      # Add admin attributes based on your user model

## 6. Create Factory Files    end

    

The slice generator expects these factories to exist:    trait :manager do

      # Add manager attributes based on your user model

Create `spec/factories/users.rb`:    end

    

```ruby    trait :member do

FactoryBot.define do      # Add member attributes based on your user model

  factory :user do    end

    email { Faker::Internet.email }  end

    name { Faker::Name.full_name }end

    

    trait :admin do# spec/factories/academies.rb

      # Add admin attributes based on your user modelFactoryBot.define do

    end  factory :academy do

        name { Faker::Company.name }

    trait :manager do    slug { Faker::Internet.slug }

      # Add manager attributes based on your user model    description { Faker::Lorem.paragraph }

    end  end

    end

    trait :member do

      # Add member attributes based on your user model# 6. Service Result Helper

    end# Create spec/support/service_result_helper.rb:

  end

endclass ServiceResult

```  attr_reader :success, :data, :errors



Create `spec/factories/academies.rb`:  def initialize(success:, data: nil, errors: [])

    @success = success

```ruby    @data = data

FactoryBot.define do    @errors = errors

  factory :academy do  end

    name { Faker::Company.name }

    slug { Faker::Internet.slug }  def success?

    description { Faker::Lorem.paragraph }    @success

  end  end

end

```  def error?

    !@success

## 7. Service Result Helper  end

end

Create `spec/support/service_result_helper.rb`:

# 7. Custom RSpec matchers for services

```ruby# Create spec/support/service_matchers.rb:

class ServiceResult

  attr_reader :success, :data, :errorsRSpec::Matchers.define :be_success do

  match do |result|

  def initialize(success:, data: nil, errors: [])    result.success?

    @success = success  end

    @data = data

    @errors = errors  failure_message do |result|

  end    "expected service result to be successful, but got errors: #{result.errors}"

  end

  def success?end

    @success

  endRSpec::Matchers.define :be_error do

  match do |result|

  def error?    result.error?

    !@success  end

  end

end  failure_message do |result|

```    "expected service result to be an error, but it was successful"

  end

## 8. Custom RSpec Matchers for Servicesend



Create `spec/support/service_matchers.rb`:# 8. Load support files

# Add this to spec/rails_helper.rb:

```ruby

RSpec::Matchers.define :be_success doDir[Rails.root.join('spec', 'support', '**', '*.rb')].sort.each { |f| require f }

  match do |result|

    result.success?# 9. Running tests

  end# Run your tests with:



  failure_message do |result|# bundle exec rspec                           # Run all tests

    "expected service result to be successful, but got errors: #{result.errors}"# bundle exec rspec spec/slices/             # Run slice tests only

  end# bundle exec rspec spec/slices/academy/     # Run specific slice tests

end# bundle exec rspec --format documentation   # Detailed output



RSpec::Matchers.define :be_error do# 10. Example .rspec configuration

  match do |result|# Create .rspec file in project root:

    result.error?

  end--require spec_helper

--format documentation

  failure_message do |result|--color

    "expected service result to be an error, but it was successful"

  end# 11. Parallel testing (optional)

end# For faster test runs, add to Gemfile:

```

group :test do

## 9. Load Support Files  gem 'parallel_tests'

end

Add this to `spec/rails_helper.rb`:

# Then run

```ruby# bundle exec parallel_rspec spec/
Dir[Rails.root.join('spec', 'support', '**', '*.rb')].sort.each { |f| require f }
```

## 10. Running Tests

Run your tests with:

```bash
bundle exec rspec                           # Run all tests
bundle exec rspec app/slices/               # Run slice tests only
bundle exec rspec app/slices/product/       # Run specific slice tests
bundle exec rspec --format documentation   # Detailed output
```

## 11. Example .rspec Configuration

Create `.rspec` file in project root:

```txt
--require spec_helper
--format documentation
--color
```

## 12. Parallel Testing (Optional)

For faster test runs, add to Gemfile:

```ruby
group :test do
  gem 'parallel_tests'
end
```

Then run:

```bash
bundle exec parallel_rspec app/slices/
```

## Slice Structure

With the updated generator, your slice structure will look like:

```txt
app/slices/product/products/
├── controllers/
├── services/
├── models/
├── serializers/
├── policies/
└── specs/                    # ← Tests are now co-located here
    ├── product_spec.rb
    ├── product_service_spec.rb
    ├── products_controller_spec.rb
    └── product_policy_spec.rb
```

This co-location keeps all slice-related code together, making it easier to understand and maintain each vertical slice.

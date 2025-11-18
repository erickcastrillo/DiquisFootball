# Authorization with Django Permissions

This document describes how authorization is implemented in Diquis using Django's permission system and custom permission classes.

## Overview

Diquis uses **Django REST Framework Permissions** combined with **Django's built-in permission system** and **custom permission classes** for authorization. This provides a flexible and powerful way to implement role-based access control (RBAC).

## Architecture

### Permission Classes

Django REST Framework uses permission classes that can be applied globally, per-view, or per-viewset.

```python
# settings.py
REST_FRAMEWORK = {
    'DEFAULT_PERMISSION_CLASSES': [
        'rest_framework.permissions.IsAuthenticated',
    ]
}
```

### Custom Permission Classes

#### IsSystemAdmin

```python
# permissions.py
from rest_framework import permissions

class IsSystemAdmin(permissions.BasePermission):
    """
    Permission class to check if user is a system administrator.
    """
    message = 'Only system administrators can perform this action.'
    
    def has_permission(self, request, view):
        return request.user and request.user.is_authenticated and request.user.is_system_admin
```

#### HasAcademyAccess

```python
class HasAcademyAccess(permissions.BasePermission):
    """
    Permission class to check if user has access to the academy.
    """
    message = 'You do not have access to this academy.'
    
    def has_permission(self, request, view):
        if not request.user or not request.user.is_authenticated:
            return False
        
        # System admins have access to all academies
        if request.user.is_system_admin:
            return True
        
        # Get academy ID from URL or header
        academy_id = self._get_academy_id(request, view)
        if not academy_id:
            return False
        
        # Check if user has access to this academy
        return AcademyUser.objects.filter(
            user=request.user,
            academy_id=academy_id,
            is_active=True
        ).exists()
    
    def _get_academy_id(self, request, view):
        # Try to get from URL kwargs
        academy_id = view.kwargs.get('academy_id')
        if academy_id:
            return academy_id
        
        # Try to get from custom header
        academy_id = request.META.get('HTTP_X_ACADEMY_CONTEXT')
        if academy_id:
            return academy_id
        
        return None
```

#### HasPermission

```python
class HasPermission(permissions.BasePermission):
    """
    Generic permission class to check specific permissions.
    """
    
    def __init__(self, required_permission):
        self.required_permission = required_permission
        super().__init__()
    
    def has_permission(self, request, view):
        if not request.user or not request.user.is_authenticated:
            return False
        
        # System admins have all permissions
        if request.user.is_system_admin:
            return True
        
        academy_id = self._get_academy_id(request, view)
        if not academy_id:
            return False
        
        # Get user's academy role
        academy_user = AcademyUser.objects.filter(
            user=request.user,
            academy_id=academy_id,
            is_active=True
        ).select_related('role').first()
        
        if not academy_user:
            return False
        
        # Check if role has the required permission
        return RolePermission.objects.filter(
            role=academy_user.role,
            permission=self.required_permission
        ).exists()
```

#### CanModifyResource

```python
class CanModifyResource(permissions.BasePermission):
    """
    Object-level permission to check if user can modify a specific resource.
    """
    
    def has_object_permission(self, request, view, obj):
        # Read permissions are allowed for any authenticated user with academy access
        if request.method in permissions.SAFE_METHODS:
            return self._has_academy_access(request.user, obj)
        
        # System admins can modify anything
        if request.user.is_system_admin:
            return True
        
        # Check if user has write permission for this academy
        return self._has_write_permission(request.user, obj)
    
    def _has_academy_access(self, user, obj):
        academy = getattr(obj, 'academy', None)
        if not academy:
            return False
        
        return AcademyUser.objects.filter(
            user=user,
            academy=academy,
            is_active=True
        ).exists()
    
    def _has_write_permission(self, user, obj):
        academy = getattr(obj, 'academy', None)
        if not academy:
            return False
        
        academy_user = AcademyUser.objects.filter(
            user=user,
            academy=academy,
            is_active=True
        ).select_related('role').first()
        
        if not academy_user:
            return False
        
        # Check if role has update permission
        return RolePermission.objects.filter(
            role=academy_user.role,
            permission='update'
        ).exists()
```

## Usage

### In ViewSets

Apply permission classes to viewsets or specific actions:

```python
from rest_framework import viewsets
from rest_framework.decorators import action
from rest_framework.response import Response

class PlayerViewSet(viewsets.ModelViewSet):
    queryset = Player.objects.all()
    serializer_class = PlayerSerializer
    permission_classes = [HasAcademyAccess]
    
    def get_queryset(self):
        academy_id = self.kwargs.get('academy_id')
        
        # System admins can see all players
        if self.request.user.is_system_admin:
            return Player.objects.filter(academy_id=academy_id)
        
        # Regular users only see players from their academy
        return Player.objects.filter(
            academy_id=academy_id,
            academy__academyuser__user=self.request.user,
            academy__academyuser__is_active=True
        )
    
    def perform_create(self, serializer):
        # Check create permission
        academy_id = self.kwargs.get('academy_id')
        if not self._has_permission('create', academy_id):
            raise PermissionDenied('You do not have permission to create players.')
        
        serializer.save(academy_id=academy_id)
    
    def perform_update(self, serializer):
        # Check update permission
        if not self._has_permission('update', serializer.instance.academy_id):
            raise PermissionDenied('You do not have permission to update this player.')
        
        serializer.save()
    
    def perform_destroy(self, instance):
        # Check delete permission
        if not self._has_permission('delete', instance.academy_id):
            raise PermissionDenied('You do not have permission to delete this player.')
        
        instance.delete()
    
    def _has_permission(self, permission, academy_id):
        if self.request.user.is_system_admin:
            return True
        
        academy_user = AcademyUser.objects.filter(
            user=self.request.user,
            academy_id=academy_id,
            is_active=True
        ).select_related('role').first()
        
        if not academy_user:
            return False
        
        return RolePermission.objects.filter(
            role=academy_user.role,
            permission=permission
        ).exists()
    
    @action(detail=True, methods=['post'], permission_classes=[IsSystemAdmin])
    def transfer(self, request, academy_id=None, pk=None):
        """Only system admins can transfer players between academies."""
        player = self.get_object()
        new_academy_id = request.data.get('new_academy_id')
        # Transfer logic...
        return Response({'message': 'Player transferred successfully'})
```

### In Function-Based Views

```python
from rest_framework.decorators import api_view, permission_classes

@api_view(['GET'])
@permission_classes([HasAcademyAccess])
def get_players(request, academy_id):
    """Get all players for an academy."""
    players = Player.objects.filter(academy_id=academy_id)
    serializer = PlayerSerializer(players, many=True)
    return Response(serializer.data)

@api_view(['POST'])
@permission_classes([HasAcademyAccess, HasPermission('create')])
def create_player(request, academy_id):
    """Create a new player."""
    serializer = PlayerSerializer(data=request.data)
    serializer.is_valid(raise_exception=True)
    serializer.save(academy_id=academy_id)
    return Response(serializer.data, status=status.HTTP_201_CREATED)
```

## Authorization Models

### AcademyUser

```python
from django.db import models
from django.contrib.auth import get_user_model
import uuid

User = get_user_model()

class AcademyUser(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    user = models.ForeignKey(User, on_delete=models.CASCADE, related_name='academy_users')
    academy = models.ForeignKey('Academy', on_delete=models.CASCADE, related_name='academy_users')
    role = models.ForeignKey('Role', on_delete=models.PROTECT, related_name='academy_users')
    is_active = models.BooleanField(default=True)
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    
    class Meta:
        db_table = 'academy_users'
        unique_together = ('user', 'academy')
    
    def __str__(self):
        return f"{self.user.email} - {self.academy.name} ({self.role.name})"
```

### Role

```python
class Role(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    name = models.CharField(max_length=50, unique=True)
    description = models.TextField(blank=True)
    created_at = models.DateTimeField(auto_now_add=True)
    
    class Meta:
        db_table = 'roles'
    
    def __str__(self):
        return self.name
```

### RolePermission

```python
class RolePermission(models.Model):
    PERMISSION_CHOICES = [
        ('create', 'Create'),
        ('read', 'Read'),
        ('update', 'Update'),
        ('delete', 'Delete'),
    ]
    
    RESOURCE_CHOICES = [
        ('player', 'Player'),
        ('team', 'Team'),
        ('training', 'Training'),
        ('asset', 'Asset'),
        ('report', 'Report'),
    ]
    
    id = models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    role = models.ForeignKey(Role, on_delete=models.CASCADE, related_name='permissions')
    permission = models.CharField(max_length=20, choices=PERMISSION_CHOICES)
    resource = models.CharField(max_length=50, choices=RESOURCE_CHOICES)
    created_at = models.DateTimeField(auto_now_add=True)
    
    class Meta:
        db_table = 'role_permissions'
        unique_together = ('role', 'permission', 'resource')
    
    def __str__(self):
        return f"{self.role.name} - {self.permission} {self.resource}"
```

## Permission Levels

### System Admin

- Full access to all academies and resources
- Can create new academies
- Can manage system-wide settings

### Academy Admin

- Full CRUD access within their academy
- Can manage academy users and roles
- Can configure academy settings

### Coach

- Read/write access to players, teams, trainings
- Cannot delete critical resources
- Limited access to academy settings

### Assistant Coach

- Read access to players and teams
- Can record training attendance
- Cannot modify rosters or schedules

### Viewer

- Read-only access to academy data
- No modification rights

## Error Handling

When authorization fails, Django REST Framework returns a 403 Forbidden response:

```python
# Custom exception handler
from rest_framework.views import exception_handler
from rest_framework.exceptions import PermissionDenied

def custom_exception_handler(exc, context):
    response = exception_handler(exc, context)
    
    if isinstance(exc, PermissionDenied):
        response.data = {
            'error': 'PERMISSION_DENIED',
            'message': 'You are not authorized to perform this action',
            'context': {
                'action': context['view'].action if hasattr(context['view'], 'action') else None,
                'resource': context['view'].basename if hasattr(context['view'], 'basename') else None,
            }
        }
    
    return response

# Add to settings.py
REST_FRAMEWORK = {
    'EXCEPTION_HANDLER': 'myapp.exceptions.custom_exception_handler'
}
```

## Testing Authorization

### Unit Tests

```python
from django.test import TestCase
from rest_framework.test import APIClient
from django.contrib.auth import get_user_model

User = get_user_model()

class PlayerAuthorizationTestCase(TestCase):
    def setUp(self):
        self.client = APIClient()
        self.admin_user = User.objects.create_user(
            email='admin@example.com',
            password='password',
            is_system_admin=True
        )
        self.regular_user = User.objects.create_user(
            email='user@example.com',
            password='password'
        )
        self.academy = Academy.objects.create(name='Test Academy')
        
    def test_create_player_without_permission_returns_forbidden(self):
        """Test that users without create permission cannot create players."""
        self.client.force_authenticate(user=self.regular_user)
        
        response = self.client.post(
            f'/api/v1/{self.academy.id}/players/',
            {'first_name': 'John', 'last_name': 'Doe'}
        )
        
        self.assertEqual(response.status_code, 403)
    
    def test_create_player_as_system_admin_succeeds(self):
        """Test that system admins can create players."""
        self.client.force_authenticate(user=self.admin_user)
        
        response = self.client.post(
            f'/api/v1/{self.academy.id}/players/',
            {'first_name': 'John', 'last_name': 'Doe', 'age': 15}
        )
        
        self.assertEqual(response.status_code, 201)
    
    def test_read_players_with_academy_access_succeeds(self):
        """Test that users with academy access can read players."""
        # Grant academy access
        AcademyUser.objects.create(
            user=self.regular_user,
            academy=self.academy,
            role=Role.objects.get(name='Viewer')
        )
        
        self.client.force_authenticate(user=self.regular_user)
        
        response = self.client.get(f'/api/v1/{self.academy.id}/players/')
        
        self.assertEqual(response.status_code, 200)
```

### Integration Tests

```python
from rest_framework.test import APITestCase

class PlayerAPITestCase(APITestCase):
    def test_full_player_workflow(self):
        """Test complete player creation and management workflow."""
        # Login
        response = self.client.post('/api/auth/login/', {
            'email': 'admin@example.com',
            'password': 'password'
        })
        token = response.data['access']
        
        # Set authentication
        self.client.credentials(HTTP_AUTHORIZATION=f'Bearer {token}')
        
        # Create player
        response = self.client.post(f'/api/v1/{academy_id}/players/', {
            'first_name': 'John',
            'last_name': 'Doe',
            'age': 15,
            'position_id': position_id
        })
        
        self.assertEqual(response.status_code, 201)
        player_id = response.data['id']
        
        # Update player
        response = self.client.patch(
            f'/api/v1/{academy_id}/players/{player_id}/',
            {'age': 16}
        )
        
        self.assertEqual(response.status_code, 200)
```

## Best Practices

### 1. Use Permission Classes

Define reusable permission classes instead of checking permissions manually in views.

### 2. Combine Permissions

Use multiple permission classes for fine-grained control:

```python
permission_classes = [IsAuthenticated, HasAcademyAccess, HasPermission('create')]
```

### 3. Object-Level Permissions

Use `has_object_permission` for resource-specific checks.

### 4. Cache Permission Checks

Use Django's caching framework to cache frequently checked permissions.

### 5. Audit Authorization Failures

Log failed authorization attempts for security monitoring.

## Security Considerations

### 1. Deny by Default

Always deny access unless explicitly granted.

### 2. Validate Academy Context

Always verify the academy context matches the user's permissions.

### 3. Check Multi-Tenancy

Ensure resources belong to the correct academy before allowing operations.

### 4. Use Database Transactions

Wrap permission checks and modifications in database transactions.

## Related Documentation

- [Django REST Framework Permissions](https://www.django-rest-framework.org/api-guide/permissions/)
- [Django Authentication and Authorization](https://docs.djangoproject.com/en/stable/topics/auth/)
- [Django Object Permissions](https://django-guardian.readthedocs.io/)
- [API_AUTHENTICATION.md](./API_AUTHENTICATION.md) - Authentication setup

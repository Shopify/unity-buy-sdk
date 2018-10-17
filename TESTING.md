## Testing

### Testing in Unity

If you want to run tests in Unity, use Unity >5.3.0.

1. Open Unity.
2. Create a Unity project within this cloned folder.
3. Click on _Window > Editor Tests Runner_.
4. In the opened Editor Tests Runner click _"Run All"_.

### Testing in Terminal using Headless Unity

Headless Unity is basically Unity running Editor tests without the GUI. When running these tests, ensure that Unity is not open.

To run tests in Headless Unity run the following command:
```bash
$ scripts/test_unity.sh
```
